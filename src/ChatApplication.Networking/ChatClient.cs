using System.Net;
using System.Net.Sockets;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;

namespace ChatApplication.Networking;

public sealed class ChatClient : IAsyncDisposable
{
    private readonly ConnectionOptions _options;
    private readonly ConnectionUpdates _updates;
    private readonly MessageUpdates _messages;
    private readonly SemaphoreSlim _lifecycle = new(1, 1);
    private readonly SemaphoreSlim _send = new(1, 1);
    private CancellationTokenSource? _lifetime;
    private TcpClient? _socket;
    private FramedConnection? _channel;
    private Task _runTask = Task.CompletedTask;
    private PendingSend? _pending;
    private long _generation;
    private bool _disposed;

    private sealed record PendingSend(long Generation, Guid SessionId, string Text)
    {
        public TaskCompletionSource<ChatMessage> Accepted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public ChatClient(Func<ConnectionSnapshot, CancellationToken, ValueTask>? observer = null,
        ConnectionOptions? options = null, Func<ChatMessage, CancellationToken, ValueTask>? onMessage = null)
    {
        _options = options ?? new();
        _options.Validate();
        _updates = new(ConnectionState.Disconnected, _options, observer);
        _messages = new(onMessage, _options);
    }

    public ConnectionSnapshot Snapshot => _updates.Current;
    public Task Completion => _runTask;

    public async Task ConnectAsync(IPEndPoint endpoint, string userName, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (endpoint.AddressFamily != AddressFamily.InterNetwork || endpoint.Port == 0)
        {
            throw new ArgumentException("An IPv4 endpoint with a nonzero port is required.", nameof(endpoint));
        }
        if (!UsernameRules.TryNormalize(userName, out var normalized))
        {
            throw new ConnectionFailureException("invalid_name");
        }

        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task run;
        await _lifecycle.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (Snapshot.State != ConnectionState.Disconnected)
            {
                throw new InvalidOperationException("Client is already active or completing a send.");
            }
            // The final Disconnected callback may still be running, or an old sender unwinding.
            // Join before replacing any generation-owned fields. Send cleanup never takes this gate.
            using var cleanup = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cleanup.CancelAfter(_options.ShutdownTimeout);
            await _runTask.WaitAsync(cleanup.Token).ConfigureAwait(false);
            await _send.WaitAsync(cleanup.Token).ConfigureAwait(false);
            _send.Release();
            _lifetime?.Dispose();
            _lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            run = _runTask = RunAsync(endpoint, normalized, ++_generation, ready, _lifetime.Token);
        }
        finally { _lifecycle.Release(); }
        try { await ready.Task.ConfigureAwait(false); }
        catch { await run.ConfigureAwait(false); throw; }
    }

    private async Task RunAsync(IPEndPoint endpoint, string name, long generation, TaskCompletionSource ready,
        CancellationToken cancellationToken)
    {
        Exception? failure = null;
        var seen = new HashSet<Guid>();
        var arrivalIds = new Queue<Guid>();
        try
        {
            await _updates.PublishAsync(new(ConnectionState.Connecting), cancellationToken).ConfigureAwait(false);
            _socket = new(AddressFamily.InterNetwork);
            using (var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                deadline.CancelAfter(_options.ConnectTimeout);
                await _socket.ConnectAsync(endpoint, deadline.Token).ConfigureAwait(false);
            }
            _channel = new(_socket.GetStream(), _options);
            await _updates.PublishAsync(new(ConnectionState.Handshaking), cancellationToken).ConfigureAwait(false);
            ConnectionPacket? welcome;
            using (var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                deadline.CancelAfter(_options.HandshakeTimeout);
                await _channel.WriteAsync(ConnectionPacket.Hello(name), deadline.Token).ConfigureAwait(false);
                welcome = await _channel.ReadAsync(PacketPhase.ClientHandshake, deadline.Token).ConfigureAwait(false);
            }
            if (welcome is null) { throw new ConnectionFailureException("connection_lost"); }
            if (welcome.Type == "error") { throw new ConnectionFailureException(welcome.Code!); }
            if (welcome.UserName != name) { throw new ConnectionFailureException("invalid_packet"); }
            await _updates.PublishAsync(new(ConnectionState.Connected, name, welcome.SessionId), cancellationToken).ConfigureAwait(false);
            ready.TrySetResult();
            while (!cancellationToken.IsCancellationRequested)
            {
                var packet = await _channel.ReadAsync(PacketPhase.ClientAccepted, cancellationToken).ConfigureAwait(false);
                if (packet is null || packet.Type == "disconnect") { break; }
                if (packet.Type == "error") { throw new ConnectionFailureException(packet.Code!); }
                var message = packet.Message!;
                if (!seen.Add(message.MessageId)) { continue; }
                arrivalIds.Enqueue(message.MessageId);
                if (arrivalIds.Count > 500) { seen.Remove(arrivalIds.Dequeue()); }
                PendingSend? pending = null;
                if (message.AuthorRole == MessageAuthorRole.Client)
                {
                    pending = Volatile.Read(ref _pending);
                    if (message.AuthorName != name || pending is null || pending.Generation != generation
                        || pending.SessionId != welcome.SessionId || pending.Text != message.Text
                        || pending.Accepted.Task.IsCompleted)
                    {
                        throw new ConnectionFailureException("invalid_packet");
                    }
                }
                await _messages.PublishAsync(message, cancellationToken).ConfigureAwait(false);
                pending?.Accepted.TrySetResult(message);
            }
        }
        catch (Exception exception)
        {
            if (!cancellationToken.IsCancellationRequested) { failure = exception; }
        }
        finally
        {
            _channel?.Dispose();
            _socket?.Dispose();
            var pending = Volatile.Read(ref _pending);
            if (pending?.Generation == generation)
            {
                pending.Accepted.TrySetException(new ConnectionFailureException("connection_lost", failure));
            }
            var code = failure is null ? null : ConnectionFailureException.CodeFor(failure);
            try { await _updates.PublishAsync(new(ConnectionState.Disconnected, ErrorCode: code)).ConfigureAwait(false); }
            catch (Exception exception) { failure ??= exception; }
            if (cancellationToken.IsCancellationRequested) { ready.TrySetCanceled(cancellationToken); }
            else if (failure is not null)
            {
                ready.TrySetException(new ConnectionFailureException(ConnectionFailureException.CodeFor(failure), failure));
            }
            else { ready.TrySetException(new ConnectionFailureException("connection_lost")); }
        }
    }

    public async Task<ChatMessage> SendAsync(string text, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!MessageRules.IsValid(text)) { throw new ArgumentException("Invalid message text.", nameof(text)); }
        FramedConnection channel;
        TcpClient socket;
        Task run;
        CancellationTokenSource source;
        CancellationToken lifetime;
        PendingSend pending;
        await _lifecycle.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var snapshot = Snapshot;
            if (snapshot.State != ConnectionState.Connected || snapshot.SessionId is not { } session
                || _channel is null || _socket is null || _lifetime is null)
            {
                throw new InvalidOperationException("No accepted server.");
            }
            if (!_send.Wait(0)) { throw new InvalidOperationException("A send is already pending."); }
            pending = new(_generation, session, text);
            Volatile.Write(ref _pending, pending);
            channel = _channel;
            socket = _socket;
            run = _runTask;
            source = _lifetime;
            lifetime = source.Token;
        }
        finally { _lifecycle.Release(); }

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime);
        deadline.CancelAfter(_options.SendTimeout);
        try
        {
            await channel.WriteAsync(ConnectionPacket.Chat(text), deadline.Token).ConfigureAwait(false);
            return await pending.Accepted.Task.WaitAsync(deadline.Token).ConfigureAwait(false);
        }
        catch
        {
            // Capture this socket/run, never a later connection's fields. No retry of uncertain text.
            source.Cancel();
            socket.Dispose();
            await run.ConfigureAwait(false);
            throw;
        }
        finally
        {
            Interlocked.CompareExchange(ref _pending, null, pending);
            _ = pending.Accepted.Task.Exception; // Observe a close racing a failed write.
            _send.Release();
        }
    }

    public void Abort()
    {
        if (_disposed) { return; }
        _lifetime?.Cancel();
        _socket?.Dispose();
    }

    public async Task DisconnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var shutdown = new CancellationTokenSource(_options.ShutdownTimeout);
        try { await _lifecycle.WaitAsync(shutdown.Token).ConfigureAwait(false); }
        catch { Abort(); throw; }
        try
        {
            if (!_runTask.IsCompleted)
            {
                var accepted = Snapshot.SessionId is not null;
                await _updates.PublishAsync(Snapshot with { State = ConnectionState.Disconnecting, ErrorCode = null }, shutdown.Token).ConfigureAwait(false);
                if (_channel is not null && accepted)
                {
                    using var deadline = CancellationTokenSource.CreateLinkedTokenSource(shutdown.Token);
                    deadline.CancelAfter(_options.SendTimeout);
                    try { await _channel.WriteAsync(ConnectionPacket.Disconnect(), deadline.Token).ConfigureAwait(false); }
                    catch (Exception exception) when (exception is IOException or SocketException
                        or OperationCanceledException or ObjectDisposedException or TimeoutException)
                    { }
                }
                Abort();
                await _runTask.WaitAsync(shutdown.Token).ConfigureAwait(false);
            }
            await _send.WaitAsync(shutdown.Token).ConfigureAwait(false);
            _send.Release();
            _channel = null;
            _socket = null;
            _lifetime?.Dispose();
            _lifetime = null;
            await _updates.PublishAsync(new(ConnectionState.Disconnected), shutdown.Token).ConfigureAwait(false);
        }
        catch { Abort(); throw; }
        finally { _lifecycle.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }
        await DisconnectAsync().ConfigureAwait(false);
        _disposed = true;
        _updates.Dispose();
        _lifecycle.Dispose();
        _send.Dispose();
    }
}
