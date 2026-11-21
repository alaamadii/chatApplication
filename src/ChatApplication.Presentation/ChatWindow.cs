using ChatApplication.Presentation.Controls;
using ChatApplication.Presentation.Layout;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation;

public abstract partial class ChatWindow : Form
{
    private readonly OwnedFonts _fonts = new();
    private readonly Panel _viewport;
    private readonly TableLayoutPanel _content;
    private readonly TableLayoutPanel _body;
    private readonly Control _conversation;
    private bool _updatingContent;

    protected OwnedFonts Fonts => _fonts;
    protected TableLayoutPanel Sidebar { get; }
    protected Label Feedback { get; }

    protected ChatWindow(string role)
    {
        SuspendLayout();
        Text = $"Chat Application - {role}";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = _fonts.Body;
        Tag = ThemeRole.Canvas;
        ClientSize = new Size(1120, 720);
        StartPosition = FormStartPosition.Manual;

        _viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Tag = ThemeRole.Canvas };
        _content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(24),
            Margin = Padding.Empty,
            Tag = ThemeRole.Canvas,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100F) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100F),
                new RowStyle(SizeType.AutoSize)
            }
        };
        _content.Controls.Add(CreateHeader(role), 0, 0);
        _content.Controls.Add(UiLayout.Text(
            "TCP connections are available. Messaging is not implemented until P03.",
            _fonts.Body, secondary: true), 0, 1);

        _body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 4, 0, 16),
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Absolute, 280F),
                new ColumnStyle(SizeType.Percent, 100F)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100F) }
        };
        var sidebarViewport = new Panel
        {
            Name = "SidebarViewport",
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Margin = Padding.Empty,
            Tag = ThemeRole.Sidebar,
            TabIndex = 0
        };
        Sidebar = UiLayout.Column(new Padding(16));
        Sidebar.Name = "ConnectionSettings";
        sidebarViewport.Controls.Add(Sidebar);

        Feedback = UiLayout.Text(
            "Connection controls are ready. Messaging arrives in P03.",
            _fonts.Metadata, secondary: true);
        Feedback.Name = "Feedback";
        Feedback.AccessibleName = "Connection and message guidance";
        _conversation = CreateConversation();
        _body.Controls.Add(sidebarViewport, 0, 0);
        _body.Controls.Add(_conversation, 1, 0);
        _content.Controls.Add(_body, 0, 2);
        _content.Controls.Add(UiLayout.Text(
            "YMH219  /  Local course demonstration  /  Message history is not saved",
            _fonts.Metadata, secondary: true), 0, 3);
        _viewport.Controls.Add(_content);
        Controls.Add(_viewport);
        _viewport.ClientSizeChanged += (_, _) => UpdateContentSize();
        Feedback.TextChanged += (_, _) => UpdateContentSize();
    }

    protected void CompleteLayout()
    {
        ChatTheme.Apply(this);
        ResumeLayout(performLayout: true);
        UpdateContentSize();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplyWorkingAreaBounds(Screen.FromControl(this).WorkingArea);
    }

    internal void ApplyWorkingAreaBounds(Rectangle workingArea)
    {
        var desired = SizeFromClientSize(new Size(ScaleLogical(1120), ScaleLogical(720)));
        var minimum = SizeFromClientSize(new Size(ScaleLogical(900), ScaleLogical(620)));
        MinimumSize = new Size(Math.Min(minimum.Width, workingArea.Width), Math.Min(minimum.Height, workingArea.Height));
        var size = new Size(Math.Min(desired.Width, workingArea.Width), Math.Min(desired.Height, workingArea.Height));
        Bounds = new Rectangle(
            workingArea.X + (workingArea.Width - size.Width) / 2,
            workingArea.Y + (workingArea.Height - size.Height) / 2,
            size.Width, size.Height);
        UpdateContentSize();
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        ApplyWorkingAreaBounds(Screen.FromControl(this).WorkingArea);
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        ChatTheme.Apply(this);
    }

    private void UpdateContentSize()
    {
        if (_content is null || _viewport is null || _conversation is null || _updatingContent)
        {
            return;
        }

        _updatingContent = true;
        try
        {
            var width = Math.Max(_viewport.ClientSize.Width, ScaleLogical(900));
            var innerWidth = width - _content.Padding.Horizontal;
            var conversationWidth = Math.Max(1, innerWidth - (int)_body.ColumnStyles[0].Width
                - _conversation.Margin.Horizontal - _conversation.Padding.Horizontal);
            var conversationHeight = _conversation.Padding.Vertical;
            foreach (Control child in _conversation.Controls)
            {
                conversationHeight += child is TranscriptView
                    ? child.MinimumSize.Height + child.Margin.Vertical
                    : PreferredHeight(child, conversationWidth);
            }

            var requiredHeight = _content.Padding.Vertical + conversationHeight
                + _body.Margin.Vertical + _conversation.Margin.Vertical;
            foreach (Control child in _content.Controls)
            {
                if (child != _body)
                {
                    requiredHeight += PreferredHeight(child, innerWidth);
                }
            }

            _content.MinimumSize = new Size(ScaleLogical(900), Math.Max(ScaleLogical(620), requiredHeight));
            _viewport.AutoScrollMinSize = _content.MinimumSize;
        }
        finally
        {
            _updatingContent = false;
        }
    }

    private static int PreferredHeight(Control control, int width) =>
        control.GetPreferredSize(new Size(Math.Max(1, width - control.Margin.Horizontal), 0)).Height
        + control.Margin.Vertical;

    private int ScaleLogical(int value) => (int)Math.Round(value * DeviceDpi / 96F);

    protected override void Dispose(bool disposing)
    {
        // Child controls release their font references before their per-window fonts.
        base.Dispose(disposing);
        if (disposing)
        {
            _fonts.Dispose();
        }
    }
}
