using System.Net;
using System.Net.Sockets;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;

namespace ChatApplication.Networking;

public sealed class ChatServer : IAsyncDisposable
{
    private readonly ConnectionOptions _options;
    private readonly MessageUpdates _messages;
    private readonly TimeProvider _clock;
    private readonly ConnectionUpdates _updates;
    private readonly SemaphoreSlim _lifecycle = new(1, 1);
    private CancellationTokenSource? _lifetime;
    private TcpListener? _listener;
    private ClientSession? _session;
    private Task _acceptTask = Task.CompletedTask;
    private Task _sessionTask = Task.CompletedTask;
    private bool _disposed;

    public ChatServer(Func<ConnectionSnapshot, CancellationToken, ValueTask>? observer = null,
        ConnectionOptions? options = null,
        Func<ChatMessage, CancellationToken, ValueTask>? onMessage = null, TimeProvider? timeProvider = null)
    {
        _options = options ?? new();
        _options.Validate();
        _updates = new(ConnectionState.Stopped, _options, observer);
        _messages = new(onMessage, _options);
        _clock = timeProvider ?? TimeProvider.System;
    }

    public ConnectionSnapshot Snapshot => _updates.Current;
    public IPEndPoint? LocalEndpoint { get; private set; }
    public Task Completion => _acceptTask;
    public Task PeerCompletion => _sessionTask;

    public async Task StartAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (endpoint.AddressFamily != AddressFamily.InterNetwork)
        {
            throw new ArgumentException("A literal IPv4 endpoint is required.", nameof(endpoint));
        }
        await _lifecycle.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!_acceptTask.IsCompleted || Snapshot.State != ConnectionState.Stopped)
            {
                throw new InvalidOperationException("Server is already active.");
            }
            _lifetime?.Dispose();
            _lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _sessionTask = Task.CompletedTask;
            _session = null;
            try
            {
                await _updates.PublishAsync(new(ConnectionState.Starting), _lifetime.Token).ConfigureAwait(false);
                _lifetime.Token.ThrowIfCancellationRequested();
                _listener = new(endpoint);
                _listener.Start(16);
                LocalEndpoint = (IPEndPoint)_listener.LocalEndpoint;
                await _updates.PublishAsync(new(ConnectionState.Listening), _lifetime.Token).ConfigureAwait(false);
                _acceptTask = AcceptLoopAsync(_listener, _lifetime.Token);
            }
            catch (Exception exception)
            {
                _listener?.Stop();
                LocalEndpoint = null;
                await _updates.PublishAsync(new(ConnectionState.Stopped,
                    ErrorCode: _lifetime.IsCancellationRequested ? null : ConnectionFailureException.CodeFor(exception))).ConfigureAwait(false);
                throw;
            }
        }
        finally { _lifecycle.Release(); }
    }

    private async Task AcceptLoopAsync(TcpListener listener, CancellationToken cancellationToken)
    {
        string? failure = null;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var socket = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                if (!_sessionTask.IsCompleted)
                {
                    // At most one rejection write is in flight; no per-arrival background task.
                    using (socket)
                    using (var channel = new FramedConnection(socket.GetStream(), _options))
                    {
                        try
                        {
                            await channel.WriteAsync(ConnectionPacket.Error("server_busy"), cancellationToken).ConfigureAwait(false);
                            using var drain = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                            drain.CancelAfter(_options.HandshakeTimeout);
                            // Consume the pending hello before close, avoiding a reset that can
                            // discard the busy response when the normal client is still writing.
                            await channel.ReadAsync(PacketPhase.ServerHandshake, drain.Token).ConfigureAwait(false);
                        }
                        catch (Exception exception) when (exception is IOException or SocketException
                            or OperationCanceledException or ObjectDisposedException or TimeoutException)
                        { }
                    }
                    continue;
                }

                await _sessionTask.ConfigureAwait(false);
                _session = new(socket, _options, _messages, _clock);
                _sessionTask = _session.RunAsync(UpdatePeerAsync, cancellationToken);
            }
        }
        catch (Exception exception)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                failure = ConnectionFailureException.CodeFor(exception);
            }
        }
        finally
        {
            _lifetime?.Cancel();
            listener.Stop();
            _session?.Dispose();
            try { await _sessionTask.ConfigureAwait(false); }
            catch (Exception exception) { failure ??= ConnectionFailureException.CodeFor(exception); }
            LocalEndpoint = null;
            if (Snapshot.State != ConnectionState.Stopping)
            {
                await _updates.PublishAsync(new(ConnectionState.Stopped, ErrorCode: failure)).ConfigureAwait(false);
            }
        }
    }

    private Task UpdatePeerAsync(string? name, Guid? session, string? error, CancellationToken token) =>
        _updates.PublishAsync(new(ConnectionState.Listening, name, session, error), token);

    public Task<ChatMessage> SendAsync(string text, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var session = _session;
        if (Snapshot.State != ConnectionState.Listening || Snapshot.SessionId is null || session is null)
        {
            throw new InvalidOperationException("No accepted peer.");
        }
        return session.DispatchAsync(text, MessageAuthorRole.Server, cancellationToken);
    }

    public void Abort()
    {
        if (_disposed) { return; }
        _lifetime?.Cancel();
        _listener?.Stop();
        _session?.Dispose();
    }

    public async Task StopAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var shutdown = new CancellationTokenSource(_options.ShutdownTimeout);
        try { await _lifecycle.WaitAsync(shutdown.Token).ConfigureAwait(false); }
        catch { Abort(); throw; }
        try
        {
            if (Snapshot.State == ConnectionState.Stopped && _acceptTask.IsCompleted) { return; }
            // Best effort is bounded and occurs before cancellation closes the stream.
            if (_session is not null && Snapshot.SessionId is not null)
            {
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(shutdown.Token);
                deadline.CancelAfter(_options.SendTimeout);
                await _session.TryDisconnectAsync(deadline.Token).ConfigureAwait(false);
            }
            Abort();
            await _updates.PublishAsync(new(ConnectionState.Stopping), shutdown.Token).ConfigureAwait(false);
            // I/O is canceled/closed first. A timeout keeps the owned task observable and
            // does not claim successful shutdown or discard it.
            await _acceptTask.WaitAsync(shutdown.Token).ConfigureAwait(false);
            _lifetime?.Dispose();
            _lifetime = null;
            _session = null;
            await _updates.PublishAsync(new(ConnectionState.Stopped), shutdown.Token).ConfigureAwait(false);
        }
        finally { _lifecycle.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }
        await StopAsync().ConfigureAwait(false);
        _disposed = true;
        _lifetime?.Dispose();
        _updates.Dispose();
        _lifecycle.Dispose();
    }
}
