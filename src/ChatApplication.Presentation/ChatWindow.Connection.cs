using ChatApplication.Presentation.Controls;

namespace ChatApplication.Presentation;

public abstract partial class ChatWindow
{
    private readonly ConnectionBadge _statusBadge = new();
    private Label _context = null!;
    private bool _closing;
    private bool _closeApproved;

    protected bool IsClosing => _closing || IsDisposed;

    protected void SetConnectionPresentation(string state, string context, ConnectionTone tone)
    {
        _statusBadge.SetStatus(state, tone);
        _context.Text = context;
        UpdateContentSize();
    }

    protected async ValueTask DispatchUpdateAsync(Action update, CancellationToken token)
    {
        if (IsClosing) { return; }
        if (!IsHandleCreated) { update(); return; }
        await InvokeAsync(() =>
        {
            if (!IsClosing) { update(); }
        }, token);
    }

    protected virtual Task CloseConnectionAsync() => Task.CompletedTask;

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        if (_closeApproved)
        {
            base.OnFormClosing(e);
            return;
        }
        e.Cancel = true;
        base.OnFormClosing(e);
        if (_closing) { return; }
        _closing = true;
        try
        {
            UpdateComposer();
            await CloseConnectionAsync();
            await _sendOperation;
            _closeApproved = true;
            Close();
        }
        catch (Exception)
        {
            _closing = false;
            Feedback.Text = "Cleanup did not complete. Try closing again; no successful shutdown is claimed.";
        }
    }
}
