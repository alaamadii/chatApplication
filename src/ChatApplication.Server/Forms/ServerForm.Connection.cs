using ChatApplication.Networking;
using ChatApplication.Presentation;
using ChatApplication.Presentation.Controls;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm
{
    private readonly ChatServer _server;
    private Task _startOperation = Task.CompletedTask;
    private Task _stopOperation = Task.CompletedTask;
    private bool _starting;
    private bool _stopping;

    private ValueTask OnConnectionAsync(ConnectionSnapshot snapshot, CancellationToken token) =>
        DispatchUpdateAsync(() => RenderConnection(snapshot), token);

    private void RenderConnection(ConnectionSnapshot snapshot)
    {
        if (IsClosing) { return; }
        var stopped = snapshot.State == ConnectionState.Stopped;
        var valid = EndpointInput.TryCreate(_bindAddress.Text, (int)_port.Value, out _);
        var editable = stopped && !_starting && !_stopping;
        _bindAddress.Enabled = editable;
        _port.Enabled = editable;
        _startButton.Enabled = editable && valid;
        _stopButton.Enabled = !_stopping && snapshot.State is ConnectionState.Starting or ConnectionState.Listening or ConnectionState.Stopping;
        var peer = snapshot.UserName is null ? "No accepted peer." : $"Accepted peer: {snapshot.UserName}.";
        RenderClientInfo(snapshot.ClientInfo);
        var tone = snapshot.ErrorCode is not null ? ConnectionTone.Error
            : snapshot.State == ConnectionState.Listening ? ConnectionTone.Success
            : stopped ? ConnectionTone.Neutral : ConnectionTone.Warning;
        SetMessagingAvailable(snapshot.State == ConnectionState.Listening && snapshot.SessionId is not null);
        SetConnectionPresentation(snapshot.State.ToString(), peer, tone);
        SetConnectionFeedback(valid || !stopped ? ConnectionFeedback.For(snapshot.ErrorCode)
            : "Enter a dotted IPv4 address and a port from 1 to 65535.");
    }

    private async void StartClicked(object? sender, EventArgs e)
    {
        if (_starting || _stopping) { return; }
        _startOperation = StartServerAsync();
        await _startOperation;
    }

    private async Task StartServerAsync()
    {
        if (!EndpointInput.TryCreate(_bindAddress.Text, (int)_port.Value, out var endpoint)) { return; }
        _starting = true;
        RenderConnection(_server.Snapshot);
        string? failure = null;
        try { await _server.StartAsync(endpoint!); }
        catch (OperationCanceledException) { }
        catch (Exception) { failure = ConnectionFeedback.For(_server.Snapshot.ErrorCode ?? "unexpected_error"); }
        finally
        {
            _starting = false;
            RenderConnection(_server.Snapshot);
            if (!IsClosing && failure is not null) { SetConnectionFeedback(failure); }
        }
    }

    private async void StopClicked(object? sender, EventArgs e)
    {
        if (_stopping) { return; }
        _stopOperation = StopServerAsync();
        await _stopOperation;
    }

    private async Task StopServerAsync()
    {
        _stopping = true;
        RenderConnection(_server.Snapshot);
        var failed = false;
        try { await _server.StopAsync(); }
        catch (Exception) { failed = true; }
        finally
        {
            _stopping = false;
            RenderConnection(_server.Snapshot);
            if (!IsClosing && failed) { SetConnectionFeedback("Server cleanup did not complete. Try Stop again."); }
        }
    }

    protected override async Task CloseConnectionAsync()
    {
        _clients.Items.Clear();
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
