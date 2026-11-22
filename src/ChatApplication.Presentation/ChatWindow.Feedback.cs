namespace ChatApplication.Presentation;

public abstract partial class ChatWindow
{
    private string _connectionGuidance = "";
    private bool _unconfirmedSend;
    private const string UnconfirmedGuidance =
        "Message was not confirmed. Text is kept; reconnect explicitly before retrying. An interrupted message may already have reached the peer.";

    protected void SetConnectionFeedback(string guidance)
    {
        _connectionGuidance = guidance;
        RefreshFeedback();
    }

    private void RefreshFeedback() =>
        Feedback.Text = _unconfirmedSend
            ? UnconfirmedGuidance + (string.IsNullOrEmpty(_connectionGuidance) ? "" : "\n" + _connectionGuidance)
            : _connectionGuidance;
}
