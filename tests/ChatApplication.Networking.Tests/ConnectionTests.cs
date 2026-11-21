using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class ConnectionTests
{
    private static readonly ConnectionOptions Options = new()
    {
        ConnectTimeout = TimeSpan.FromSeconds(2),
        HandshakeTimeout = TimeSpan.FromSeconds(1),
        FrameTimeout = TimeSpan.FromSeconds(1),
        SendTimeout = TimeSpan.FromSeconds(1),
        ShutdownTimeout = TimeSpan.FromSeconds(2)
    };

    [Fact]
    public async Task Real_handshake_disconnect_reconnect_and_server_restart_release_owned_work()
    {
        var serverEvents = new Probe();
        await using var server = new ChatServer(serverEvents.Observe, Options);
        await using var client = new ChatClient(options: Options);
        var sessions = new HashSet<Guid>();
        for (var restart = 0; restart < 2; restart++)
        {
            await server.StartAsync(new(IPAddress.Loopback, 0));
            Assert.Equal(ConnectionState.Listening, server.Snapshot.State);
            for (var index = 0; index < 3; index++)
            {
                await client.ConnectAsync(server.LocalEndpoint!, "  علاء  ");
                var accepted = await serverEvents.WaitAsync(value => value.SessionId is not null);
                Assert.Equal("علاء", accepted.UserName);
                Assert.Equal(ConnectionState.Connected, client.Snapshot.State);
                Assert.Equal(accepted.SessionId, client.Snapshot.SessionId);
                Assert.True(sessions.Add(client.Snapshot.SessionId!.Value));
                await client.DisconnectAsync();
                await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
                Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
                Assert.Null(server.Snapshot.UserName);
                Assert.True(client.Completion.IsCompletedSuccessfully);
            }
            await server.StopAsync();
            Assert.Null(server.LocalEndpoint);
            Assert.True(server.Completion.IsCompletedSuccessfully);
        }
    }

    [Fact]
    public async Task Additional_client_is_rejected_without_disrupting_accepted_peer()
    {
        await using var server = new ChatServer(options: Options);
        await using var first = new ChatClient(options: Options);
        await using var second = new ChatClient(options: Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await first.ConnectAsync(server.LocalEndpoint!, "First");
        var failure = await Assert.ThrowsAsync<ConnectionFailureException>(() =>
            second.ConnectAsync(server.LocalEndpoint!, "Second"));
        Assert.Equal("server_busy", failure.Code);
        Assert.Equal(ConnectionState.Connected, first.Snapshot.State);
        Assert.True(second.Completion.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Pending_handshake_reserves_slot_then_timeout_releases_it()
    {
        var events = new Probe();
        await using var server = new ChatServer(events.Observe, Options with { HandshakeTimeout = TimeSpan.FromMilliseconds(300) });
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var pending = new TcpClient();
        await pending.ConnectAsync(server.LocalEndpoint!);
        await pending.GetStream().WriteAsync(new byte[] { 0 });
        using var rejected = new TcpClient();
        await rejected.ConnectAsync(server.LocalEndpoint!);
        using var rejection = new FramedConnection(rejected.GetStream(), Options);
        Assert.Equal("server_busy", (await rejection.ReadAsync(PacketPhase.ClientHandshake))!.Code);
        rejection.Dispose();
        rejected.Dispose();
        await events.WaitAsync(value => value.ErrorCode == "operation_timeout");
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        await using var valid = new ChatClient(options: Options);
        await valid.ConnectAsync(server.LocalEndpoint!, "Valid");
        Assert.Equal(ConnectionState.Connected, valid.Snapshot.State);
    }

    [Theory]
    [InlineData("{\"version\":1,\"type\":\"hello\",\"userName\":\"Server\"}", "invalid_name")]
    [InlineData("{\"version\":2,\"type\":\"hello\",\"userName\":\"Valid\"}", "unsupported_version")]
    [InlineData("{\"version\":1,\"type\":\"welcome\",\"userName\":\"Valid\"}", "invalid_packet")]
    public async Task Server_rejects_bad_handshake_and_clears_peer(string json, string code)
    {
        await using var server = new ChatServer(options: Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var raw = new TcpClient();
        await raw.ConnectAsync(server.LocalEndpoint!);
        await raw.GetStream().WriteAsync(ProtocolTests.Frame(json));
        using var channel = new FramedConnection(raw.GetStream(), Options);
        Assert.Equal(code, (await channel.ReadAsync(PacketPhase.ClientHandshake))!.Code);
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Null(server.Snapshot.SessionId);
        Assert.Equal(ConnectionState.Listening, server.Snapshot.State);
    }

    [Fact]
    public async Task Client_does_not_report_connected_before_welcome_and_cancel_joins_read()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        await using var client = new ChatClient(options: Options);
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Valid");
        using var peer = await listener.AcceptTcpClientAsync();
        using var channel = new FramedConnection(peer.GetStream(), Options);
        Assert.Equal("hello", (await channel.ReadAsync(PacketPhase.ServerHandshake))!.Type);
        Assert.Equal(ConnectionState.Handshaking, client.Snapshot.State);
        Assert.False(connect.IsCompleted);
        await client.DisconnectAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => connect);
        Assert.True(client.Completion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
        Assert.Null(client.Snapshot.ErrorCode);
    }

    [Fact]
    public async Task Handshake_timeout_cleans_up_the_connect_operation()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        await using var client = new ChatClient(options: Options with { HandshakeTimeout = TimeSpan.FromMilliseconds(100) });
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Valid");
        using var peer = await listener.AcceptTcpClientAsync();
        using var channel = new FramedConnection(peer.GetStream(), Options);
        Assert.Equal("hello", (await channel.ReadAsync(PacketPhase.ServerHandshake))!.Type);
        Assert.Equal("operation_timeout", (await Assert.ThrowsAsync<ConnectionFailureException>(() => connect)).Code);
        Assert.True(client.Completion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
    }

    [Fact]
    public async Task Client_rejects_mismatched_welcome_and_unknown_remote_error_text()
    {
        foreach (var response in new[]
        {
            ProtocolTests.Frame(MessageProtocol.Encode(ConnectionPacket.Welcome("Other", Guid.NewGuid()))),
            ProtocolTests.Frame("{\"version\":1,\"type\":\"error\",\"code\":\"arbitrary remote text\"}")
        })
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            await using var client = new ChatClient(options: Options);
            var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, "Valid");
            using var peer = await listener.AcceptTcpClientAsync();
            using var channel = new FramedConnection(peer.GetStream(), Options);
            await channel.ReadAsync(PacketPhase.ServerHandshake);
            await peer.GetStream().WriteAsync(response);
            Assert.Equal("invalid_packet", (await Assert.ThrowsAsync<ConnectionFailureException>(() => connect)).Code);
            Assert.True(client.Completion.IsCompletedSuccessfully);
        }
    }

    [Fact]
    public async Task Occupied_port_and_refused_endpoint_are_recoverable_and_do_not_claim_connection()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var endpoint = (IPEndPoint)listener.LocalEndpoint;
        await using var server = new ChatServer(options: Options);
        await Assert.ThrowsAsync<SocketException>(() => server.StartAsync(endpoint));
        Assert.Equal(ConnectionState.Stopped, server.Snapshot.State);
        Assert.Equal("address_in_use", server.Snapshot.ErrorCode);
        listener.Stop();
        await using var client = new ChatClient(options: Options);
        var refusal = await Assert.ThrowsAsync<ConnectionFailureException>(() => client.ConnectAsync(endpoint, "Valid"));
        Assert.Contains(refusal.Code, new[] { "connection_refused", "operation_timeout" });
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
        Assert.True(client.Completion.IsCompletedSuccessfully);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "Valid");
        await server.StopAsync();
        await client.Completion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
    }

    [Fact]
    public async Task Stop_during_incomplete_prefix_joins_peer_and_accept_tasks()
    {
        await using var server = new ChatServer(options: Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var raw = new TcpClient();
        await raw.ConnectAsync(server.LocalEndpoint!);
        await raw.GetStream().WriteAsync(new byte[] { 0, 0 });
        await server.StopAsync();
        Assert.True(server.PeerCompletion.IsCompleted);
        Assert.True(server.Completion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Stopped, server.Snapshot.State);
    }

    [Fact]
    public async Task Cancelled_connect_token_never_creates_a_live_client()
    {
        await using var client = new ChatClient(options: Options);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.ConnectAsync(new(IPAddress.Loopback, 1), "Valid", cancellation.Token));
        Assert.True(client.Completion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
    }

    private sealed class Probe
    {
        private readonly Channel<ConnectionSnapshot> _events = Channel.CreateUnbounded<ConnectionSnapshot>();
        public ValueTask Observe(ConnectionSnapshot value, CancellationToken token) =>
            _events.Writer.WriteAsync(value, token);

        public async Task<ConnectionSnapshot> WaitAsync(Func<ConnectionSnapshot, bool> predicate)
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            while (true)
            {
                var value = await _events.Reader.ReadAsync(deadline.Token);
                if (predicate(value)) { return value; }
            }
        }
    }
}
