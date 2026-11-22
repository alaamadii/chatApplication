using System.Net;
using System.Net.Sockets;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class LifecycleRegressionTests
{
    private static readonly ConnectionOptions Options = new()
    {
        ConnectTimeout = TimeSpan.FromSeconds(2),
        HandshakeTimeout = TimeSpan.FromSeconds(2),
        FrameTimeout = TimeSpan.FromSeconds(2),
        SendTimeout = TimeSpan.FromSeconds(2),
        ShutdownTimeout = TimeSpan.FromSeconds(2)
    };
    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public async Task Failed_disconnecting_notification_still_aborts_and_joins_client_read()
    {
        await using var server = new ChatServer(options: Options);
        await using var client = new ChatClient((state, _) =>
        {
            if (state.State == ConnectionState.Disconnecting) { throw new InvalidOperationException("Synthetic observer failure."); }
            return ValueTask.CompletedTask;
        }, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "Alaa");
        try
        {
            await Assert.ThrowsAsync<ConnectionFailureException>(() => client.DisconnectAsync());
            await client.Completion.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(ConnectionState.Disconnected, client.Snapshot.State);
        }
        finally { client.Abort(); }
    }

    [Fact]
    public async Task Explicit_reconnect_waits_for_previous_disconnected_notification_to_finish()
    {
        var entered = Signal();
        var release = Signal();
        var waitOnce = true;
        await using var server = new ChatServer(options: Options);
        await using var client = new ChatClient(async (state, token) =>
        {
            if (state.State == ConnectionState.Disconnected && waitOnce)
            {
                waitOnce = false;
                entered.TrySetResult();
                await release.Task.WaitAsync(token);
            }
        }, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        await client.ConnectAsync(server.LocalEndpoint!, "First");
        client.Abort();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        await server.PeerCompletion.WaitAsync(TimeSpan.FromSeconds(3));
        try
        {
            var reconnect = client.ConnectAsync(server.LocalEndpoint!, "Second");
            Assert.False(reconnect.IsCompleted);
            release.TrySetResult();
            await reconnect.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal("Second", client.Snapshot.UserName);
            Assert.Equal(ConnectionState.Connected, client.Snapshot.State);
        }
        finally { release.TrySetResult(); client.Abort(); }
    }

    [Fact]
    public async Task Explicit_start_waits_for_previous_stopped_notification_to_finish()
    {
        var entered = Signal();
        var release = Signal();
        var waitOnce = true;
        await using var server = new ChatServer(async (state, token) =>
        {
            if (state.State == ConnectionState.Stopped && waitOnce)
            {
                waitOnce = false;
                entered.TrySetResult();
                await release.Task.WaitAsync(token);
            }
        }, Options);
        await server.StartAsync(new(IPAddress.Loopback, 0));
        server.Abort();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        try
        {
            var restart = server.StartAsync(new(IPAddress.Loopback, 0));
            Assert.False(restart.IsCompleted);
            release.TrySetResult();
            await restart.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(ConnectionState.Listening, server.Snapshot.State);
        }
        finally { release.TrySetResult(); server.Abort(); }
    }
}
