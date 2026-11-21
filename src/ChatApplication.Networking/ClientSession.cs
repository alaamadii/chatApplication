using System.Net.Sockets;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;

namespace ChatApplication.Networking;

internal sealed class ClientSession(TcpClient socket, ConnectionOptions options) : IDisposable
{
    private readonly FramedConnection _channel = new(socket.GetStream(), options);

    public async Task RunAsync(
        Func<string?, Guid?, string?, CancellationToken, Task> update, CancellationToken cancellationToken)
    {
        string? failure = null;
        try
        {
            using var handshake = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            handshake.CancelAfter(options.HandshakeTimeout);
            ConnectionPacket? hello;
            try { hello = await _channel.ReadAsync(PacketPhase.ServerHandshake, handshake.Token).ConfigureAwait(false); }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
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
            await update(name, sessionId, null, cancellationToken).ConfigureAwait(false);
            var close = await _channel.ReadAsync(PacketPhase.ServerAccepted, cancellationToken).ConfigureAwait(false);
            // Only disconnect is supported after welcome in P02; the codec rejects other packets.
            if (close is not null && close.Type != "disconnect")
            {
                throw new ConnectionFailureException("invalid_packet");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception exception)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                failure = ConnectionFailureException.CodeFor(exception);
                if (failure is "invalid_name" or "invalid_packet" or "unsupported_version")
                {
                    try { await _channel.WriteAsync(ConnectionPacket.Error(failure), cancellationToken).ConfigureAwait(false); }
                    catch (Exception sendFailure) when (sendFailure is IOException or SocketException
                        or OperationCanceledException or ObjectDisposedException or TimeoutException)
                    { }
                }
            }
        }
        finally
        {
            Dispose();
            if (!cancellationToken.IsCancellationRequested)
            {
                await update(null, null, failure, cancellationToken).ConfigureAwait(false);
            }
        }
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
        _channel.Dispose();
        socket.Dispose();
    }
}
