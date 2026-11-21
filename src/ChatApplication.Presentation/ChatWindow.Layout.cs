using ChatApplication.Presentation.Controls;
using ChatApplication.Presentation.Layout;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation;

public abstract partial class ChatWindow
{
    private Control CreateHeader(string role)
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 12),
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100F),
                new ColumnStyle(SizeType.AutoSize)
            }
        };
        var identity = UiLayout.Column();
        var title = UiLayout.Text("Chat Application", _fonts.Title);
        title.Margin = new Padding(0, 0, 0, 8);
        UiLayout.AddRow(identity, title);
        UiLayout.AddRow(identity, UiLayout.Text(
            role == "Server" ? "SERVER  /  Listener and client overview" : "CLIENT  /  Your connection and conversation",
            _fonts.Metadata, secondary: true));
        header.Controls.Add(identity, 0, 0);
        _statusBadge.Font = _fonts.Metadata;
        _statusBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        header.Controls.Add(_statusBadge, 1, 0);
        return header;
    }

    private Control CreateConversation()
    {
        var conversation = new TableLayoutPanel
        {
            Name = "Conversation",
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(16),
            Margin = new Padding(16, 0, 0, 0),
            Tag = ThemeRole.Surface,
            TabIndex = 1,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100F) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100F),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize)
            }
        };
        var heading = UiLayout.Column();
        UiLayout.AddRow(heading, UiLayout.Text("Conversation", _fonts.Section));
        _context = UiLayout.Text("No active connection.", _fonts.Metadata, secondary: true);
        UiLayout.AddRow(heading, _context);
        conversation.Controls.Add(heading, 0, 0);
        _transcript = new TranscriptView(_fonts);
        conversation.Controls.Add(_transcript, 0, 1);
        conversation.Controls.Add(CreateComposer(), 0, 2);
        Feedback.Margin = Padding.Empty;
        conversation.Controls.Add(Feedback, 0, 3);
        return conversation;
    }

    private Control CreateComposer()
    {
        var composer = UiLayout.Column();
        composer.Name = "Composer";
        var text = _messageInput = new MessageComposer
        {
            Name = "MessageInput",
            AccessibleName = "Message",
            AccessibleDescription = "Enter sends, Shift+Enter adds a line, Tab moves focus. Available after connection; 2000 UTF-16 units maximum.",
            Multiline = true,
            Enabled = false,
            Height = 64,
            ScrollBars = ScrollBars.Vertical,
            Font = _fonts.Body
        };
        UiLayout.AddRow(composer, UiLayout.Field("&Message", text));
        var action = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8),
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100F),
                new ColumnStyle(SizeType.Absolute, 104F)
            }
        };
        var hint = _lengthHint = UiLayout.Text("2000 remaining · Enter: send · Shift+Enter: new line", _fonts.Metadata, secondary: true);
        hint.TextAlign = ContentAlignment.MiddleLeft;
        hint.Margin = new Padding(0, 0, 12, 0);
        var send = _sendButton = UiLayout.ActionButton("&Send", "SendMessage");
        text.TextChanged += (_, _) => UpdateComposer();
        text.SendRequested += RequestSend;
        send.Click += RequestSend;
        send.Margin = Padding.Empty;
        action.Controls.Add(hint, 0, 0);
        action.Controls.Add(send, 1, 0);
        UiLayout.AddRow(composer, action);
        return composer;
    }
}
