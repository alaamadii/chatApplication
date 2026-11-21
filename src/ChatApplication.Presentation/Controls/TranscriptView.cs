using ChatApplication.Core;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Controls;

public sealed class TranscriptView : UserControl
{
    public const int MaximumMessages = 500;
    private readonly OwnedFonts _fonts;
    private readonly Panel _viewport;
    private readonly Panel _content;
    private readonly Label _empty;
    private readonly Label _summary;
    private readonly Label _notice;
    private readonly List<MessageCard> _cards = [];
    private readonly HashSet<Guid> _ids = [];
    private bool _layingOut;
    private int _lastWidth;

    public int MessageCount => _cards.Count;
    public bool HasTrimmedMessages { get; private set; }

    public TranscriptView(OwnedFonts fonts)
    {
        _fonts = fonts;
        Name = "Transcript";
        AccessibleName = "Conversation";
        AccessibleDescription = "Messages in server acceptance order. Latest 500 only; never saved.";
        Dock = DockStyle.Fill;
        MinimumSize = new Size(0, 96);
        Margin = new Padding(0, 0, 0, 16);
        Tag = ThemeRole.Surface;
        TabStop = false;
        _summary = new Label
        {
            Name = "MessageCount",
            Dock = DockStyle.Top,
            Font = fonts.Metadata,
            Text = "0 messages",
            Height = 24,
            Visible = false,
            UseMnemonic = false
        };
        _notice = new Label
        {
            Name = "TrimNotice",
            Dock = DockStyle.Bottom,
            Font = fonts.Metadata,
            Text = "Older messages were removed. Only the latest 500 are kept in this window.",
            Visible = false,
            UseMnemonic = false
        };
        _empty = new Label
        {
            Name = "EmptyConversation",
            Dock = DockStyle.Fill,
            Font = fonts.Body,
            Text = "Your conversation starts here\nConnect to exchange text messages. Nothing is saved.",
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false
        };
        _content = new Panel { Dock = DockStyle.Top, Height = 0, Tag = ThemeRole.Surface };
        _viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Tag = ThemeRole.Surface };
        _viewport.Controls.Add(_content);
        _viewport.Controls.Add(_empty);
        Controls.Add(_viewport);
        Controls.Add(_summary);
        Controls.Add(_notice);
        _viewport.ClientSizeChanged += (_, _) => Reflow();
    }

    public void Append(ChatMessage message, bool outgoing)
    {
        if (!_ids.Add(message.MessageId)) { return; }
        var trimmed = false;
        var card = new MessageCard(message, outgoing, _fonts);
        _cards.Add(card);
        _content.Controls.Add(card);
        if (_cards.Count > MaximumMessages)
        {
            var removed = _cards[0];
            _cards.RemoveAt(0);
            _ids.Remove(removed.Message.MessageId);
            _content.Controls.Remove(removed);
            removed.Dispose();
            HasTrimmedMessages = true;
            trimmed = true;
        }
        _empty.Visible = false;
        _summary.Visible = true;
        _summary.Text = $"{_cards.Count} messages";
        _notice.Visible = HasTrimmedMessages;
        Reflow(force: true, startIndex: trimmed ? 0 : _cards.Count - 1);
        _viewport.ScrollControlIntoView(card);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        _lastWidth = 0;
        Reflow();
    }

    private void Reflow(bool force = false, int startIndex = 0)
    {
        if (_viewport is null || _layingOut) { return; }
        var width = Math.Max(1, _viewport.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
        if (!force && width == _lastWidth) { return; }
        if (width != _lastWidth) { startIndex = 0; }
        _lastWidth = width;
        _layingOut = true;
        _content.SuspendLayout();
        try
        {
            // Reserve scrollbar space consistently, avoiding width/height feedback.
            var gap = (int)Math.Round(8 * DeviceDpi / 96F);
            _content.Width = width;
            var y = startIndex > 0 ? _cards[startIndex - 1].Bottom + gap : 0;
            for (var i = startIndex; i < _cards.Count; i++)
            {
                var card = _cards[i];
                card.FitWidth(Math.Max(1, width * 9 / 10));
                card.Location = new Point(card.Outgoing ? width - card.Width : 0, y);
                y += card.Height + gap;
            }
            _content.Height = y;
            _notice.Height = TextRenderer.MeasureText(_notice.Text, _notice.Font,
                new Size(Math.Max(1, ClientSize.Width), int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height;
        }
        finally { _content.ResumeLayout(); _layingOut = false; }
    }
}
