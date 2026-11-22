using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class PeerInformationTests
{
    private static readonly ConnectionOptions Options = new()
    {
        ConnectTimeout = TimeSpan.FromSeconds(2),
        HandshakeTimeout = TimeSpan.FromSeconds(2),
        FrameTimeout = TimeSpan.FromSeconds(2),
        SendTimeout = TimeSpan.FromSeconds(2),
        ShutdownTimeout = TimeSpan.FromSeconds(2)
    };
    private sealed class Clock : TimeProvider
    {
        private long _seconds;
        public override DateTimeOffset GetUtcNow() => DateTimeOffset.UnixEpoch.AddSeconds(Interlocked.Increment(ref _seconds));
    }
    private sealed class States
    {
        private readonly Channel<ConnectionSnapshot> _events = Channel.CreateUnbounded<ConnectionSnapshot>();
        public ValueTask Observe(ConnectionSnapshot state, CancellationToken token) => _events.Writer.WriteAsync(state, token);
        public async Task<ConnectionSnapshot> Wait(Func<ConnectionSnapshot, bool> predicate)
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            while (true)
            {
                var state = await _events.Reader.ReadAsync(deadline.Token);
                if (predicate(state)) { return state; }
            }
        }
    }

    [Fact]
    public async Task Metadata_comes_from_accepted_socket_and_clock_and_busy_peer_cannot_replace_it()
    {
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options, timeProvider: new Clock());
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var socket = new TcpClient();
        await socket.ConnectAsync(server.LocalEndpoint!);
        var local = (IPEndPoint)socket.Client.LocalEndPoint!;
        using var wire = new FramedConnection(socket.GetStream(), Options);
        Assert.Null(server.Snapshot.ClientInfo);
        await wire.WriteAsync(ConnectionPacket.Hello("  Alaa  "));
        var welcome = await wire.ReadAsync(PacketPhase.ClientHandshake);
        var accepted = await states.Wait(state => state.ClientInfo is not null);
        var info = accepted.ClientInfo!;
        Assert.Equal(welcome!.SessionId, info.SessionId);
        Assert.Equal("Alaa", info.UserName);
        Assert.Equal(local.Address.MapToIPv4().ToString(), info.RemoteAddress);
        Assert.Equal(local.Port, info.RemotePort);
        Assert.Equal(new IPEndPoint(local.Address.MapToIPv4(), local.Port).ToString(), info.RemoteEndpoint);
        Assert.Equal(DateTimeOffset.UnixEpoch.AddSeconds(1), info.ConnectedAtUtc);
        Assert.Equal(ConnectionState.Connected, info.State);
        Assert.Equal(info.UserName, accepted.UserName);
        Assert.Equal(info.SessionId, accepted.SessionId);
        await using var busy = new ChatClient(options: Options);
        Assert.Equal("server_busy", (await Assert.ThrowsAsync<ConnectionFailureException>(() =>
            busy.ConnectAsync(server.LocalEndpoint!, "Other"))).Code);
        Assert.Same(info, server.Snapshot.ClientInfo);
        await wire.WriteAsync(ConnectionPacket.Disconnect());
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Null(server.Snapshot.ClientInfo);
        Assert.Null(server.Snapshot.SessionId);
        Assert.Null(server.Snapshot.UserName);

        await using var fresh = new ChatClient(options: Options);
        await fresh.ConnectAsync(server.LocalEndpoint!, "Fresh");
        var replacement = (await states.Wait(state => state.UserName == "Fresh")).ClientInfo!;
        Assert.NotEqual(info.SessionId, replacement.SessionId);
        Assert.True(replacement.ConnectedAtUtc > info.ConnectedAtUtc);
        Assert.Equal("Fresh", replacement.UserName);
        await server.StopAsync();
        await fresh.Completion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Null(server.Snapshot.ClientInfo);
    }

    [Fact]
    public async Task Pending_and_rejected_handshakes_never_publish_accepted_metadata()
    {
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var pending = new TcpClient();
        await pending.ConnectAsync(server.LocalEndpoint!);
        await pending.GetStream().WriteAsync(new byte[] { 0 });
        await using var busy = new ChatClient(options: Options);
        Assert.Equal("server_busy", (await Assert.ThrowsAsync<ConnectionFailureException>(() =>
            busy.ConnectAsync(server.LocalEndpoint!, "Other"))).Code);
        Assert.Null(server.Snapshot.ClientInfo);
        pending.Dispose();
        await states.Wait(state => state.ErrorCode is not null);
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        using var invalid = new TcpClient();
        await invalid.ConnectAsync(server.LocalEndpoint!);
        using var wire = new FramedConnection(invalid.GetStream(), Options);
        await wire.WriteAsync(ConnectionPacket.Hello("Server"));
        Assert.Equal("invalid_name", (await wire.ReadAsync(PacketPhase.ClientHandshake))!.Code);
        await states.Wait(state => state.ErrorCode == "invalid_name");
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Null(server.Snapshot.ClientInfo);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Stop_during_partial_input_clears_metadata_and_joins_all_owned_work(bool accepted)
    {
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var socket = new TcpClient();
        await socket.ConnectAsync(server.LocalEndpoint!);
        using var wire = new FramedConnection(socket.GetStream(), Options);
        if (accepted)
        {
            await wire.WriteAsync(ConnectionPacket.Hello("Alaa"));
            await wire.ReadAsync(PacketPhase.ClientHandshake);
            await states.Wait(state => state.ClientInfo is not null);
        }
        await socket.GetStream().WriteAsync(new byte[] { 0, 0 });
        await server.StopAsync();
        Assert.Null(server.Snapshot.ClientInfo);
        Assert.True(server.Completion.IsCompletedSuccessfully);
        Assert.True(server.PeerCompletion.IsCompletedSuccessfully);
        Assert.Equal(ConnectionState.Stopped, server.Snapshot.State);
    }

    [Fact]
    public async Task Abrupt_socket_reset_clears_live_info_and_explicit_new_peer_can_send()
    {
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        using var socket = new TcpClient();
        await socket.ConnectAsync(server.LocalEndpoint!);
        using var wire = new FramedConnection(socket.GetStream(), Options);
        await wire.WriteAsync(ConnectionPacket.Hello("First"));
        await wire.ReadAsync(PacketPhase.ClientHandshake);
        var first = (await states.Wait(state => state.ClientInfo is not null)).ClientInfo!;
        await wire.WriteAsync(ConnectionPacket.Chat("before reset"));
        Assert.Equal("before reset", (await wire.ReadAsync(PacketPhase.ClientAccepted))!.Message!.Text);
        socket.Client.LingerState = new(true, 0);
        socket.Dispose();
        await states.Wait(state => state.ClientInfo is null);
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(ConnectionState.Listening, server.Snapshot.State);
        Assert.Null(server.Snapshot.ClientInfo);
        await using var next = new ChatClient(options: Options);
        await next.ConnectAsync(server.LocalEndpoint!, "Second");
        var second = (await states.Wait(state => state.UserName == "Second")).ClientInfo!;
        Assert.NotEqual(first.SessionId, second.SessionId);
        Assert.Equal("Second", (await next.SendAsync("after reset")).AuthorName);
        Assert.Same(second, server.Snapshot.ClientInfo);
    }

    [Fact]
    public async Task Simultaneous_disconnect_stop_and_repeated_restart_do_not_leave_stale_sessions()
    {
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options);
        await using var client = new ChatClient(options: Options);
        var ids = new HashSet<Guid>();
        for (var index = 0; index < 12; index++)
        {
            await server.StartAsync(new(IPAddress.Loopback, 0));
            await client.ConnectAsync(server.LocalEndpoint!, "Alaa");
            var current = (await states.Wait(state => state.ClientInfo is not null)).ClientInfo!;
            Assert.True(ids.Add(current.SessionId));
            await client.SendAsync($"cycle {index}");
            await Task.WhenAll(client.DisconnectAsync(), server.StopAsync()).WaitAsync(TimeSpan.FromSeconds(4));
            Assert.Null(server.Snapshot.ClientInfo);
            Assert.Equal(ConnectionState.Stopped, server.Snapshot.State);
            Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
            Assert.True(server.Completion.IsCompletedSuccessfully && server.PeerCompletion.IsCompletedSuccessfully,
                $"Accept={server.Completion.Status}; peer={server.PeerCompletion.Status}; failure={server.PeerCompletion.Exception}");
            Assert.True(client.Completion.IsCompletedSuccessfully);
        }
    }

    [Fact]
    public async Task Cleanup_timeout_retains_observable_work_and_does_not_claim_stopped()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var states = new States();
        await using var server = new ChatServer(states.Observe, Options with { ShutdownTimeout = TimeSpan.FromMilliseconds(250) },
            onMessage: async (_, _) =>
            {
                entered.TrySetResult();
                await release.Task; // Deliberately broken observer, released by this fixture.
            });
        await using var client = new ChatClient(options: Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "Alaa");
        await states.Wait(state => state.ClientInfo is not null);
        await client.SendAsync("active observer");
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => server.StopAsync());
            Assert.Equal(ConnectionState.Stopping, server.Snapshot.State);
            Assert.Null(server.Snapshot.ClientInfo);
            Assert.False(server.Completion.IsCompleted);
            Assert.False(server.PeerCompletion.IsCompleted);
        }
        finally { release.TrySetResult(); }
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(3));
        await server.StopAsync();
        Assert.Equal(ConnectionState.Stopped, server.Snapshot.State);
        Assert.True(server.Completion.IsCompletedSuccessfully && server.PeerCompletion.IsCompletedSuccessfully);
    }
}
