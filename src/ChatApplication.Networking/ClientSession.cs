using System.Net.Sockets;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;

namespace ChatApplication.Networking;

// One gate orders operator and peer messages, including their two presentation callbacks.
internal sealed class ClientSession(
    TcpClient socket, ConnectionOptions options, MessageUpdates messages, TimeProvider clock) : IDisposable
{
    private readonly FramedConnection _channel = new(socket.GetStream(), options);
    private readonly SemaphoreSlim _dispatch = new(1, 1);
    private readonly CancellationTokenSource _closed = new();
    private string? _name;
    private bool _accepted;

    public async Task RunAsync(
        Func<string?, Guid?, string?, CancellationToken, Task> update, CancellationToken cancellationToken)
    {
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _closed.Token);
        var token = lifetime.Token;
        string? failure = null;
        try
        {
            using var handshake = CancellationTokenSource.CreateLinkedTokenSource(token);
            handshake.CancelAfter(options.HandshakeTimeout);
            ConnectionPacket? hello;
            try { hello = await _channel.ReadAsync(PacketPhase.ServerHandshake, handshake.Token).ConfigureAwait(false); }
            catch (OperationCanceledException exception) when (!token.IsCancellationRequested)
            {
                throw new TimeoutException("Handshake deadline exceeded.", exception);
            }
            if (hello is null) { return; }
            if (!UsernameRules.TryNormalize(hello.UserName, out var name))
            {
                throw new ConnectionFailureException("invalid_name");
            }

            var sessionId = Guid.NewGuid();
            await _channel.WriteAsync(ConnectionPacket.Welcome(name, sessionId), handshake.Token).ConfigureAwait(false);
            _name = name;
            _accepted = true;
            await update(name, sessionId, null, token).ConfigureAwait(false);
            while (!token.IsCancellationRequested)
            {
                var packet = await _channel.ReadAsync(PacketPhase.ServerAccepted, token).ConfigureAwait(false);
                if (packet is null || packet.Type == "disconnect") { break; }
                await DispatchAsync(packet.Text!, MessageAuthorRole.Client, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception exception)
        {
            if (!token.IsCancellationRequested)
            {
                failure = ConnectionFailureException.CodeFor(exception);
                if (failure is "invalid_name" or "invalid_packet" or "unsupported_version")
                {
                    try { await _channel.WriteAsync(ConnectionPacket.Error(failure), token).ConfigureAwait(false); }
                    catch (Exception sendFailure) when (sendFailure is IOException or SocketException
                        or OperationCanceledException or ObjectDisposedException or TimeoutException)
                    { }
                }
            }
        }
        finally
        {
            Dispose();
            // Release the peer slot only after an operator dispatch has unwound.
            await _dispatch.WaitAsync().ConfigureAwait(false);
            _dispatch.Release();
            if (!cancellationToken.IsCancellationRequested)
            {
                await update(null, null, failure, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public async Task<ChatMessage> DispatchAsync(string text, MessageAuthorRole role, CancellationToken cancellationToken)
    {
        if (!MessageRules.IsValid(text)) { throw new ArgumentException("Invalid message text.", nameof(text)); }
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _closed.Token);
        deadline.CancelAfter(options.SendTimeout);
        var entered = false;
        try
        {
            await _dispatch.WaitAsync(deadline.Token).ConfigureAwait(false);
            entered = true;
            if (!_accepted || _name is null) { throw new InvalidOperationException("No accepted peer."); }
            deadline.Token.ThrowIfCancellationRequested();
            var message = new ChatMessage(Guid.NewGuid(), role,
                role == MessageAuthorRole.Server ? "Server" : _name, text, clock.GetUtcNow());
            await _channel.WriteAsync(ConnectionPacket.AcceptedMessage(message), deadline.Token).ConfigureAwait(false);
            await messages.PublishAsync(message, deadline.Token).ConfigureAwait(false);
            return message;
        }
        catch
        {
            // A partial write or failed display is uncertain: end this session, never retry it.
            Dispose();
            throw;
        }
        finally { if (entered) { _dispatch.Release(); } }
    }

    public async Task TryDisconnectAsync(CancellationToken cancellationToken)
    {
        try { await _channel.WriteAsync(ConnectionPacket.Disconnect(), cancellationToken).ConfigureAwait(false); }
        catch (Exception exception) when (exception is IOException or SocketException
            or OperationCanceledException or ObjectDisposedException or TimeoutException)
        { }
    }

    public void Dispose()
    {
        _accepted = false;
        _closed.Cancel();
        _channel.Dispose();
        socket.Dispose();
        // The semaphore/CTS have no native wait handle and remain valid while async owners unwind.
    }
}
