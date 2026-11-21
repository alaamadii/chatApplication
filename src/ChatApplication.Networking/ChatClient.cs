using System.Net;
using System.Net.Sockets;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;

namespace ChatApplication.Networking;

public sealed class ChatClient : IAsyncDisposable
{
    private readonly ConnectionOptions _options;
    private readonly ConnectionUpdates _updates;
    private readonly SemaphoreSlim _lifecycle = new(1, 1);
    private CancellationTokenSource? _lifetime;
    private TcpClient? _socket;
    private FramedConnection? _channel;
    private Task _runTask = Task.CompletedTask;
    private bool _disposed;

    public ChatClient(Func<ConnectionSnapshot, CancellationToken, ValueTask>? observer = null,
        ConnectionOptions? options = null)
    {
        _options = options ?? new();
        _options.Validate();
        _updates = new(ConnectionState.Disconnected, _options, observer);
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
            if (!_runTask.IsCompleted) { throw new InvalidOperationException("Client is already active."); }
            _lifetime?.Dispose();
            _lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            run = _runTask = RunAsync(endpoint, normalized, ready, _lifetime.Token);
        }
        finally { _lifecycle.Release(); }
        try { await ready.Task.ConfigureAwait(false); }
        catch
        {
            await run.ConfigureAwait(false);
            throw;
        }
    }

    private async Task RunAsync(IPEndPoint endpoint, string name, TaskCompletionSource ready,
        CancellationToken cancellationToken)
    {
        Exception? failure = null;
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
            var close = await _channel.ReadAsync(PacketPhase.ClientAccepted, cancellationToken).ConfigureAwait(false);
            if (close?.Type == "error") { throw new ConnectionFailureException(close.Code!); }
        }
        catch (Exception exception)
        {
            if (!cancellationToken.IsCancellationRequested) { failure = exception; }
        }
        finally
        {
            _channel?.Dispose();
            _socket?.Dispose();
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
            _channel = null;
            _socket = null;
            _lifetime?.Dispose();
            _lifetime = null;
            await _updates.PublishAsync(new(ConnectionState.Disconnected), shutdown.Token).ConfigureAwait(false);
        }
        finally { _lifecycle.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }
        await DisconnectAsync().ConfigureAwait(false);
        _disposed = true;
        _updates.Dispose();
        _lifecycle.Dispose();
    }
}
