using ChatApplication.Networking;
using ChatApplication.Presentation;
using ChatApplication.Presentation.Controls;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm
{
    private readonly ChatServer _server;
    private Task _startOperation = Task.CompletedTask;
    private Task _stopOperation = Task.CompletedTask;

    private ValueTask OnConnectionAsync(ConnectionSnapshot snapshot, CancellationToken token) =>
        DispatchUpdateAsync(() => RenderConnection(snapshot), token);

    private void RenderConnection(ConnectionSnapshot snapshot)
    {
        if (IsClosing) { return; }
        var stopped = snapshot.State == ConnectionState.Stopped;
        var valid = EndpointInput.TryCreate(_bindAddress.Text, (int)_port.Value, out _);
        _bindAddress.Enabled = stopped;
        _port.Enabled = stopped;
        _startButton.Enabled = stopped && valid;
        _stopButton.Enabled = snapshot.State is ConnectionState.Starting or ConnectionState.Listening;
        var peer = snapshot.UserName is null ? "No accepted peer." : $"Accepted peer: {snapshot.UserName}.";
        _peerSummary.Text = peer + " Detailed client information arrives in P04.";
        _clients.AccessibleDescription = _peerSummary.Text;
        var tone = snapshot.ErrorCode is not null ? ConnectionTone.Error
            : snapshot.State == ConnectionState.Listening ? ConnectionTone.Success
            : stopped ? ConnectionTone.Neutral : ConnectionTone.Warning;
        SetMessagingAvailable(snapshot.State == ConnectionState.Listening && snapshot.SessionId is not null);
        SetConnectionPresentation(snapshot.State.ToString(), peer, tone);
        Feedback.Text = valid || !stopped ? ConnectionFeedback.For(snapshot.ErrorCode)
            : "Enter a dotted IPv4 address and a port from 1 to 65535.";
    }

    private async void StartClicked(object? sender, EventArgs e)
    {
        _startOperation = StartServerAsync();
        await _startOperation;
    }

    private async Task StartServerAsync()
    {
        if (!EndpointInput.TryCreate(_bindAddress.Text, (int)_port.Value, out var endpoint)) { return; }
        try { await _server.StartAsync(endpoint!); }
        catch (Exception) { if (!IsClosing) { Feedback.Text = ConnectionFeedback.For(_server.Snapshot.ErrorCode); } }
        finally { RenderConnection(_server.Snapshot); }
    }

    private async void StopClicked(object? sender, EventArgs e)
    {
        _stopOperation = StopServerAsync();
        await _stopOperation;
    }

    private async Task StopServerAsync()
    {
        try { await _server.StopAsync(); }
        catch (Exception) { if (!IsClosing) { Feedback.Text = "Server cleanup did not complete. Try Stop again."; } }
        finally { RenderConnection(_server.Snapshot); }
    }

    protected override async Task CloseConnectionAsync()
    {
        _server.Abort();
        await Task.WhenAll(_startOperation, _stopOperation);
        await _server.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _server?.Abort(); }
        base.Dispose(disposing);
    }
}
