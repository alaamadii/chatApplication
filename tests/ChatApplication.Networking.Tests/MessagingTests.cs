using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class MessagingTests
{
    private static readonly ConnectionOptions Options = new()
    {
        ConnectTimeout = TimeSpan.FromSeconds(2),
        HandshakeTimeout = TimeSpan.FromSeconds(2),
        FrameTimeout = TimeSpan.FromSeconds(2),
        SendTimeout = TimeSpan.FromSeconds(2),
        ShutdownTimeout = TimeSpan.FromSeconds(3)
    };
    private sealed class FrozenClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => DateTimeOffset.Parse("2026-11-21T12:34:56Z");
    }
    private sealed class Messages
    {
        private readonly Channel<ChatMessage> _queue = Channel.CreateUnbounded<ChatMessage>();
        public ValueTask Observe(ChatMessage message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Assert.True(_queue.Writer.TryWrite(message));
            return ValueTask.CompletedTask;
        }
        public async Task<ChatMessage> Next() => await _queue.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(4));
        public bool IsEmpty => !_queue.Reader.TryPeek(out _);
    }

    [Fact]
    public async Task Bidirectional_identical_unicode_messages_have_one_shared_identity_time_and_order()
    {
        var serverMessages = new Messages();
        var clientMessages = new Messages();
        var accepted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = new ChatServer((state, _) =>
        {
            if (state.SessionId is not null) { accepted.TrySetResult(); }
            return ValueTask.CompletedTask;
        }, Options, serverMessages.Observe, new FrozenClock());
        await using var client = new ChatClient(options: Options, onMessage: clientMessages.Observe);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "Alaa");
        await accepted.Task.WaitAsync(TimeSpan.FromSeconds(3));
        const string text = "مرحبا & Türkçe 👋\r\nsecond\tline";
        var first = await client.SendAsync(text);
        var second = await client.SendAsync(text);
        var both = await Task.WhenAll(client.SendAsync("client concurrent"), server.SendAsync("operator concurrent"));
        var received = new List<ChatMessage>();
        for (var i = 0; i < 4; i++)
        {
            var local = await serverMessages.Next();
            Assert.Equal(local, await clientMessages.Next());
            Assert.Equal(new FrozenClock().GetUtcNow(), local.SentAtUtc);
            received.Add(local);
        }
        Assert.Equal(4, received.Select(message => message.MessageId).Distinct().Count());
        Assert.Equal(text, first.Text);
        Assert.NotEqual(first.MessageId, second.MessageId);
        Assert.Equal("Alaa", first.AuthorName);
        Assert.Equal("Server", both.Single(message => message.AuthorRole == MessageAuthorRole.Server).AuthorName);
        Assert.True(serverMessages.IsEmpty && clientMessages.IsEmpty);
        await client.DisconnectAsync();
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        await client.ConnectAsync(server.LocalEndpoint!, "Other");
        var fresh = await client.SendAsync(text);
        Assert.Equal("Other", fresh.AuthorName);
        Assert.NotEqual(first.MessageId, fresh.MessageId);
        Assert.Equal(fresh, await serverMessages.Next());
        Assert.Equal(fresh, await clientMessages.Next());
    }

    [Fact]
    public async Task Raw_client_cannot_spoof_author_and_wrong_direction_ends_session()
    {
        var messages = new Messages();
        await using var server = new ChatServer(options: Options, onMessage: messages.Observe);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var socket = new TcpClient();
        await socket.ConnectAsync(server.LocalEndpoint!);
        using var wire = new FramedConnection(socket.GetStream(), Options);
        await wire.WriteAsync(ConnectionPacket.Hello("Actual"));
        await wire.ReadAsync(PacketPhase.ClientHandshake);
        await socket.GetStream().WriteAsync(ProtocolTests.Frame(
            """{"version":1,"type":"chat","text":"literal","authorName":"Victim","authorRole":"server","sentAtUtc":"fake"}"""));
        var echo = (await wire.ReadAsync(PacketPhase.ClientAccepted))!.Message!;
        Assert.Equal("Actual", echo.AuthorName);
        Assert.Equal(MessageAuthorRole.Client, echo.AuthorRole);
        Assert.Equal(echo, await messages.Next());
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(echo));
        Assert.Equal("invalid_packet", (await wire.ReadAsync(PacketPhase.ClientAccepted))!.Code);
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Null(server.Snapshot.SessionId);
        Assert.True(messages.IsEmpty);
    }

    [Theory]
    [InlineData("close")]
    [InlineData("timeout")]
    [InlineData("cancel")]
    [InlineData("disconnect")]
    public async Task Pending_send_failure_is_joined_and_reconnect_uses_a_fresh_session(string mode)
    {
        var messages = new Messages();
        var options = Options with { SendTimeout = TimeSpan.FromMilliseconds(500) };
        await using var client = new ChatClient(options: options, onMessage: messages.Observe);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Alaa");
        using var socket = await listener.AcceptTcpClientAsync();
        using var wire = new FramedConnection(socket.GetStream(), Options);
        await wire.ReadAsync(PacketPhase.ServerHandshake);
        await wire.WriteAsync(ConnectionPacket.Welcome("Alaa", Guid.NewGuid()));
        await connect;
        using var cancellation = new CancellationTokenSource();
        var send = client.SendAsync("repeat", cancellation.Token);
        Assert.Equal("repeat", (await wire.ReadAsync(PacketPhase.ServerAccepted))!.Text);
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.SendAsync("parallel"));
        if (mode == "close") { socket.Dispose(); }
        if (mode == "cancel") { cancellation.Cancel(); }
        if (mode == "disconnect") { await client.DisconnectAsync(); }
        await Assert.ThrowsAnyAsync<Exception>(() => send.WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.True(send.IsCompleted);
        await client.Completion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
        Assert.True(messages.IsEmpty);

        await using var real = new ChatServer(options: Options);
        await real.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(real.LocalEndpoint!, "Fresh");
        var result = await client.SendAsync("repeat");
        Assert.Equal("Fresh", result.AuthorName);
        Assert.Equal(result, await messages.Next());
        Assert.True(messages.IsEmpty);
    }

    [Fact]
    public async Task Unrelated_operator_message_does_not_confirm_pending_client_text()
    {
        var messages = new Messages();
        await using var client = new ChatClient(options: Options, onMessage: messages.Observe);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Alaa");
        using var socket = await listener.AcceptTcpClientAsync();
        using var wire = new FramedConnection(socket.GetStream(), Options);
        await wire.ReadAsync(PacketPhase.ServerHandshake);
        await wire.WriteAsync(ConnectionPacket.Welcome("Alaa", Guid.NewGuid()));
        await connect;
        var send = client.SendAsync("same");
        await wire.ReadAsync(PacketPhase.ServerAccepted);
        var unrelated = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Server, "Server", "same", DateTimeOffset.UnixEpoch);
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(unrelated));
        Assert.Equal(unrelated, await messages.Next());
        Assert.False(send.IsCompleted);
        var accepted = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Client, "Alaa", "same", DateTimeOffset.UnixEpoch);
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(accepted));
        Assert.Equal(accepted, await send);
        Assert.Equal(accepted, await messages.Next());
        // Repeated server IDs are ignored without suppressing distinct equal text.
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(accepted));
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(unrelated with { }));
        var next = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Server, "Server", "barrier", DateTimeOffset.UnixEpoch);
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(next));
        Assert.Equal(next, await messages.Next());
        Assert.True(messages.IsEmpty);
    }

    [Fact]
    public async Task Server_stop_cancels_and_joins_active_message_observer_before_reuse()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = new ChatServer(options: Options, onMessage: async (_, token) =>
        {
            entered.TrySetResult();
            try { await Task.Delay(Timeout.Infinite, token); }
            finally { exited.TrySetResult(); }
        });
        await using var client = new ChatClient(options: Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "Alaa");
        await client.SendAsync("active");
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        await server.StopAsync();
        Assert.True(exited.Task.IsCompletedSuccessfully);
        Assert.True(server.PeerCompletion.IsCompletedSuccessfully);
        Assert.True(server.Completion.IsCompletedSuccessfully);
        await client.Completion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
    }

    [Fact]
    public async Task Acceptance_timeout_cancels_inflight_display_before_pending_send_completes()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var client = new ChatClient(options: Options with { SendTimeout = TimeSpan.FromMilliseconds(500) },
            onMessage: async (_, token) =>
            {
                entered.TrySetResult();
                try { await Task.Delay(Timeout.Infinite, token); }
                finally { exited.TrySetResult(); }
            });
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Alaa");
        using var socket = await listener.AcceptTcpClientAsync();
        using var wire = new FramedConnection(socket.GetStream(), Options);
        await wire.ReadAsync(PacketPhase.ServerHandshake);
        await wire.WriteAsync(ConnectionPacket.Welcome("Alaa", Guid.NewGuid()));
        await connect;
        var send = client.SendAsync("pending display");
        await wire.ReadAsync(PacketPhase.ServerAccepted);
        await wire.WriteAsync(ConnectionPacket.AcceptedMessage(new(Guid.NewGuid(), MessageAuthorRole.Client,
            "Alaa", "pending display", DateTimeOffset.UnixEpoch)));
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.ThrowsAnyAsync<Exception>(() => send.WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.True(exited.Task.IsCompletedSuccessfully);
        Assert.True(client.Completion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
    }
}
