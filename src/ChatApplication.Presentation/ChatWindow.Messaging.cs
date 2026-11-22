using ChatApplication.Core;
using ChatApplication.Presentation.Controls;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation;

public abstract partial class ChatWindow
{
    private MessageComposer _messageInput = null!;
    private Button _sendButton = null!;
    private Label _lengthHint = null!;
    private TranscriptView _transcript = null!;
    private Task _sendOperation = Task.CompletedTask;
    private bool _messagingAvailable;
    private bool _sending;
    private readonly bool _isServer;

    protected bool IsSending => _sending;
    protected virtual void OnMessageSendFinished() { }

    protected void SetMessagingAvailable(bool available)
    {
        _messagingAvailable = available;
        UpdateComposer();
    }

    protected void AppendMessage(ChatMessage message) =>
        _transcript.Append(message, _isServer == (message.AuthorRole == MessageAuthorRole.Server));

    protected virtual Task<ChatMessage> SendMessageAsync(string text) =>
        Task.FromException<ChatMessage>(new InvalidOperationException("Messaging is unavailable."));

    private void UpdateComposer()
    {
        if (_messageInput is null) { return; }
        _messageInput.Enabled = _messagingAvailable && !_sending && !IsClosing;
        _sendButton.Enabled = _messageInput.Enabled && MessageRules.IsValid(_messageInput.Text);
        _lengthHint.Text = _sending ? "Waiting for server acceptance..."
            : $"{MessageRules.MaximumLength - _messageInput.TextLength} remaining · Enter: send · Shift+Enter: new line";
        ChatTheme.Apply(_messageInput);
    }

    private async void RequestSend(object? sender, EventArgs e)
    {
        if (_sending || !_sendButton.Enabled || IsClosing) { return; }
        _sendOperation = SendFromComposerAsync(_messageInput.Text);
        await _sendOperation;
    }

    private async Task SendFromComposerAsync(string submitted)
    {
        _sending = true;
        UpdateComposer();
        try
        {
            await SendMessageAsync(submitted);
            if (!IsClosing)
            {
                if (_messageInput.Text == submitted) { _messageInput.Clear(); }
                _unconfirmedSend = false;
                RefreshFeedback();
            }
        }
        catch (Exception)
        {
            if (!IsClosing)
            {
                _unconfirmedSend = true;
                RefreshFeedback();
            }
        }
        finally
        {
            _sending = false;
            if (!IsClosing) { UpdateComposer(); OnMessageSendFinished(); }
        }
    }
}
