using System.Buffers.Binary;

namespace ChatApplication.Networking.Protocol;

// One reader owns ReadAsync. The writer gate covers both prefix and payload.
public sealed class FramedConnection : IDisposable
{
    private readonly Stream _stream;
    private readonly ConnectionOptions _options;
    private readonly SemaphoreSlim _writer = new(1, 1);

    public FramedConnection(Stream stream, ConnectionOptions? options = null)
    {
        _stream = stream;
        _options = options ?? new();
        _options.Validate();
    }

    public async Task<ConnectionPacket?> ReadAsync(PacketPhase phase, CancellationToken cancellationToken = default)
    {
        var prefix = new byte[4];
        var first = await _stream.ReadAsync(prefix.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
        if (first == 0)
        {
            return null;
        }

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(_options.FrameTimeout);
        try
        {
            await ReadRemainderAsync(prefix.AsMemory(1), deadline.Token).ConfigureAwait(false);
            var length = BinaryPrimitives.ReadInt32BigEndian(prefix);
            if (length is < 1 or > MessageProtocol.MaximumPayloadBytes)
            {
                throw new ConnectionFailureException("invalid_packet");
            }

            var payload = new byte[length];
            await ReadRemainderAsync(payload, deadline.Token).ConfigureAwait(false);
            return MessageProtocol.Decode(payload, phase);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _stream.Dispose();
            throw new TimeoutException("Frame completion deadline exceeded.", exception);
        }
    }

    public async Task WriteAsync(ConnectionPacket packet, CancellationToken cancellationToken = default)
    {
        var payload = MessageProtocol.Encode(packet);
        var frame = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(frame, payload.Length);
        payload.CopyTo(frame, 4);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(_options.SendTimeout);
        var entered = false;
        try
        {
            await _writer.WaitAsync(deadline.Token).ConfigureAwait(false);
            entered = true;
            await _stream.WriteAsync(frame, deadline.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _stream.Dispose();
            throw new TimeoutException("Frame write deadline exceeded.", exception);
        }
        catch (OperationCanceledException)
        {
            _stream.Dispose();
            throw;
        }
        finally
        {
            if (entered) { _writer.Release(); }
        }
    }

    private async Task ReadRemainderAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        while (!buffer.IsEmpty)
        {
            var count = await _stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (count == 0) { throw new ConnectionFailureException("invalid_packet"); }
            buffer = buffer[count..];
        }
    }

    // Stream/socket disposal unblocks pending I/O. Semaphore is not disposed while
    // a canceled writer may still be unwinding; it has no allocated wait handle.
    public void Dispose() => _stream.Dispose();
}
