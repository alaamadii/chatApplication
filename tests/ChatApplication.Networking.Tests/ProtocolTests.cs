using System.Buffers.Binary;
using System.Text;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class ProtocolTests
{
    internal static byte[] Frame(string json) => Frame(Encoding.UTF8.GetBytes(json));
    internal static byte[] Frame(byte[] payload)
    {
        var frame = new byte[payload.Length + 4];
        BinaryPrimitives.WriteInt32BigEndian(frame, payload.Length);
        payload.CopyTo(frame, 4);
        return frame;
    }

    [Fact]
    public async Task Reads_bytewise_and_coalesced_frames_without_losing_boundaries()
    {
        var data = Frame("{\"version\":1,\"type\":\"hello\",\"userName\":\"علاء\"}")
            .Concat(Frame("{\"version\":1,\"type\":\"hello\",\"userName\":\"Çağrı\"}")).ToArray();
        using var stream = new FragmentedStream(data);
        using var reader = new FramedConnection(stream);
        Assert.Equal("علاء", (await reader.ReadAsync(PacketPhase.ServerHandshake))!.UserName);
        Assert.Equal("Çağrı", (await reader.ReadAsync(PacketPhase.ServerHandshake))!.UserName);
        Assert.Null(await reader.ReadAsync(PacketPhase.ServerHandshake));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(16385)]
    [InlineData(int.MaxValue)]
    public async Task Rejects_invalid_prefix_before_reading_payload(int length)
    {
        var bytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(bytes, length);
        using var stream = new MemoryStream(bytes);
        using var channel = new FramedConnection(stream);
        Assert.Equal("invalid_packet", (await Assert.ThrowsAsync<ConnectionFailureException>(
            () => channel.ReadAsync(PacketPhase.ServerHandshake))).Code);
        Assert.Equal(4, stream.Position);
    }

    [Fact]
    public async Task Accepts_exact_maximum_payload_and_prefix_is_encoded_byte_length()
    {
        const string start = "{\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\",\"extra\":\"";
        var json = start + new string('x', 16384 - start.Length - 2) + "\"}";
        using var channel = new FramedConnection(new MemoryStream(Frame(json)));
        Assert.Equal("Aa", (await channel.ReadAsync(PacketPhase.ServerHandshake))!.UserName);

        using var output = new MemoryStream();
        using var writer = new FramedConnection(output);
        await writer.WriteAsync(ConnectionPacket.Hello("علاء"));
        var frame = output.ToArray();
        Assert.Equal(frame.Length - 4, BinaryPrimitives.ReadInt32BigEndian(frame));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(12)]
    public async Task Distinguishes_truncated_frames_from_clean_EOF(int count)
    {
        var bytes = Frame("{\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\"}")[..count];
        using var channel = new FramedConnection(new MemoryStream(bytes));
        await Assert.ThrowsAsync<ConnectionFailureException>(() => channel.ReadAsync(PacketPhase.ServerHandshake));
    }

    [Theory]
    [InlineData("[]", "invalid_packet")]
    [InlineData("null", "invalid_packet")]
    [InlineData("{\"version\":2,\"type\":\"hello\",\"userName\":\"Aa\"}", "unsupported_version")]
    [InlineData("{\"version\":1,\"type\":\"chat\",\"text\":\"future\"}", "invalid_packet")]
    [InlineData("{\"version\":1,\"type\":\"welcome\",\"userName\":\"Aa\"}", "invalid_packet")]
    [InlineData("{\"version\":1,\"type\":\"hello\",\"userName\":42}", "invalid_packet")]
    [InlineData("{\"version\":1,\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\"}", "invalid_packet")]
    [InlineData("{\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\",\"extra\":{\"x\":1,\"x\":2}}", "invalid_packet")]
    [InlineData("{\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\",\"extra\":[[[[[[[[[0]]]]]]]]]}", "invalid_packet")]
    [InlineData("{\"version\":1,\"type\":\"hello\",\"userName\":\"Aa\"}{}", "invalid_packet")]
    public void Rejects_invalid_JSON_version_duplicates_depth_fields_and_direction(string json, string code) =>
        Assert.Equal(code, Assert.Throws<ConnectionFailureException>(() =>
            MessageProtocol.Decode(Encoding.UTF8.GetBytes(json), PacketPhase.ServerHandshake)).Code);

    [Fact]
    public void Rejects_invalid_UTF8_and_bad_welcome_identity()
    {
        Assert.Throws<ConnectionFailureException>(() =>
            MessageProtocol.Decode([0xff, 0xfe], PacketPhase.ServerHandshake));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(
            Encoding.UTF8.GetBytes("{\"version\":1,\"type\":\"welcome\",\"sessionId\":\"bad\",\"userName\":\"Aa\"}"),
            PacketPhase.ClientHandshake));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(
            MessageProtocol.Encode(ConnectionPacket.Hello("Aa")), PacketPhase.ClientAccepted));
    }

    [Fact]
    public async Task Concurrent_writes_keep_complete_frames_intact()
    {
        using var stream = new YieldingWriteStream();
        using var channel = new FramedConnection(stream);
        await Task.WhenAll(Enumerable.Range(0, 25).Select(index => channel.WriteAsync(ConnectionPacket.Hello("User" + index))));
        stream.Position = 0;
        var names = new HashSet<string>();
        for (var index = 0; index < 25; index++)
        {
            names.Add((await channel.ReadAsync(PacketPhase.ServerHandshake))!.UserName!);
        }
        Assert.Equal(25, names.Count);
        Assert.Null(await channel.ReadAsync(PacketPhase.ServerHandshake));
    }

    [Fact]
    public async Task Partial_frame_timeout_cancels_underlying_read_and_closes_stream()
    {
        using var stream = new BlockingReadStream(firstByte: true);
        using var channel = new FramedConnection(stream, new() { FrameTimeout = TimeSpan.FromMilliseconds(40) });
        await Assert.ThrowsAsync<TimeoutException>(() => channel.ReadAsync(PacketPhase.ServerHandshake));
        Assert.True(stream.ReadUnwound);
        Assert.True(stream.Disposed);
    }

    [Fact]
    public async Task Idle_read_waits_for_explicit_cancellation_instead_of_frame_timeout()
    {
        using var stream = new BlockingReadStream(firstByte: false);
        using var channel = new FramedConnection(stream, new() { FrameTimeout = TimeSpan.FromMilliseconds(10) });
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(80));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            channel.ReadAsync(PacketPhase.ServerAccepted, cancellation.Token));
        Assert.True(stream.ReadUnwound);
    }

    [Fact]
    public async Task Write_deadline_unwinds_and_closes_the_underlying_stream()
    {
        using var stream = new BlockingWriteStream();
        using var channel = new FramedConnection(stream, new() { SendTimeout = TimeSpan.FromMilliseconds(40) });
        await Assert.ThrowsAsync<TimeoutException>(() => channel.WriteAsync(ConnectionPacket.Hello("Valid")));
        Assert.True(stream.Unwound);
        Assert.True(stream.Disposed);
    }

    [Theory]
    [InlineData("127.0.0.1", 5000, true)]
    [InlineData("010.0.0.1", 5000, true)]
    [InlineData("127.1", 5000, false)]
    [InlineData("0x7f.0.0.1", 5000, false)]
    [InlineData("localhost", 5000, false)]
    [InlineData("::1", 5000, false)]
    [InlineData("256.0.0.1", 5000, false)]
    [InlineData("127.0.0.1", 0, false)]
    public void Production_endpoint_input_requires_four_decimal_IPv4_octets_and_nonzero_port(
        string address, int port, bool expected)
    {
        Assert.Equal(expected, EndpointInput.TryCreate(address, port, out var endpoint));
        if (address.StartsWith("010.") && expected) { Assert.Equal("10.0.0.1", endpoint!.Address.ToString()); }
    }

    private sealed class BlockingWriteStream : MemoryStream
    {
        public bool Unwound { get; private set; }
        public bool Disposed { get; private set; }
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            try { await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken); }
            finally { Unwound = true; }
        }
        protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
    }

    private sealed class FragmentedStream(byte[] bytes) : MemoryStream(bytes)
    {
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            base.ReadAsync(buffer[..Math.Min(1, buffer.Length)], cancellationToken);
    }

    private sealed class YieldingWriteStream : MemoryStream
    {
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            foreach (var value in buffer.ToArray())
            {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
                WriteByte(value);
            }
        }
    }

    private sealed class BlockingReadStream(bool firstByte) : MemoryStream
    {
        private bool _returnedFirst;
        public bool ReadUnwound { get; private set; }
        public bool Disposed { get; private set; }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (firstByte && !_returnedFirst)
            {
                _returnedFirst = true;
                buffer.Span[0] = 0;
                return 1;
            }
            try { await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken); return 0; }
            finally { ReadUnwound = true; }
        }

        protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
    }
}
