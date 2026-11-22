using ChatApplication.Core;
using ChatApplication.Networking;
using ChatApplication.Presentation;
using ChatApplication.Presentation.Controls;

namespace ChatApplication.Client.Forms;

internal sealed partial class ClientForm
{
    private readonly ChatClient _client;
    private Task _connectOperation = Task.CompletedTask;
    private Task _disconnectOperation = Task.CompletedTask;
    private bool _connecting;
    private bool _disconnecting;

    private ValueTask OnConnectionAsync(ConnectionSnapshot snapshot, CancellationToken token) =>
        DispatchUpdateAsync(() => RenderConnection(snapshot), token);

    private void RenderConnection(ConnectionSnapshot snapshot)
    {
        if (IsClosing) { return; }
        var disconnected = snapshot.State == ConnectionState.Disconnected;
        var validEndpoint = EndpointInput.TryCreate(_serverAddress.Text, (int)_port.Value, out _);
        var validName = UsernameRules.TryNormalize(_username.Text, out _);
        var editable = disconnected && !_connecting && !_disconnecting && !IsSending;
        _username.Enabled = _serverAddress.Enabled = _port.Enabled = editable;
        _connectButton.Enabled = editable && validEndpoint && validName;
        var pending = snapshot.State is ConnectionState.Connecting or ConnectionState.Handshaking;
        _disconnectButton.Text = pending ? "C&ancel" : "&Disconnect";
        _disconnectButton.AccessibleName = pending ? "Cancel connection" : "Disconnect";
        _disconnectButton.Enabled = !_disconnecting && (pending || snapshot.State is ConnectionState.Connected or ConnectionState.Disconnecting);
        var context = snapshot.UserName is null ? "No active connection." : $"Connected as {snapshot.UserName}.";
        var tone = snapshot.ErrorCode is not null ? ConnectionTone.Error
            : snapshot.State == ConnectionState.Connected ? ConnectionTone.Success
            : disconnected ? ConnectionTone.Neutral : ConnectionTone.Warning;
        SetMessagingAvailable(snapshot.State == ConnectionState.Connected);
        SetConnectionPresentation(snapshot.State.ToString(), context, tone);
        SetConnectionFeedback(snapshot.ErrorCode is not null ? ConnectionFeedback.For(snapshot.ErrorCode)
            : disconnected && !validName ? "Enter a supported username before connecting."
            : disconnected && !validEndpoint ? "Enter a dotted IPv4 address and a port from 1 to 65535."
            : ConnectionFeedback.For(null));
    }

    private async void ConnectClicked(object? sender, EventArgs e)
    {
        if (_connecting || _disconnecting || IsSending) { return; }
        _connectOperation = ConnectClientAsync();
        await _connectOperation;
    }

    private async Task ConnectClientAsync()
    {
        if (!EndpointInput.TryCreate(_serverAddress.Text, (int)_port.Value, out var endpoint)) { return; }
        _connecting = true;
        RenderConnection(_client.Snapshot);
        string? failure = null;
        try { await _client.ConnectAsync(endpoint!, _username.Text); }
        catch (OperationCanceledException) { }
        catch (Exception) { failure = ConnectionFeedback.For(_client.Snapshot.ErrorCode ?? "unexpected_error"); }
        finally
        {
            _connecting = false;
            RenderConnection(_client.Snapshot);
            if (!IsClosing && failure is not null) { SetConnectionFeedback(failure); }
        }
    }

    private async void DisconnectClicked(object? sender, EventArgs e)
    {
        if (_disconnecting) { return; }
        _disconnectOperation = DisconnectClientAsync();
        await _disconnectOperation;
    }

    private async Task DisconnectClientAsync()
    {
        _disconnecting = true;
        RenderConnection(_client.Snapshot);
        var failed = false;
        try { await _client.DisconnectAsync(); }
        catch (Exception) { failed = true; }
        finally
        {
            _disconnecting = false;
            RenderConnection(_client.Snapshot);
            if (!IsClosing && failed) { SetConnectionFeedback("Client cleanup did not complete. Try Disconnect again."); }
        }
    }

    protected override async Task CloseConnectionAsync()
    {
        _client.Abort();
        await Task.WhenAll(_connectOperation, _disconnectOperation);
        await _client.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _client?.Abort(); }
        base.Dispose(disposing);
    }
}
