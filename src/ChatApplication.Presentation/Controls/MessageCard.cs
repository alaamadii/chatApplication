using ChatApplication.Core;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Controls;

// Borrows per-window fonts. Labels always render literal plain text.
public sealed class MessageCard : Panel
{
    private readonly Label _author;
    private readonly Label _time;
    private readonly Label _body;
    private int _measuredWidth;
    private int _measuredDpi;
    public ChatMessage Message { get; }
    public bool Outgoing { get; }

    public MessageCard(ChatMessage message, bool outgoing, OwnedFonts fonts)
    {
        Message = message;
        Outgoing = outgoing;
        Name = "MessageCard";
        AccessibleName = $"Message from {message.AuthorName}";
        TabStop = false;
        _author = CreateLabel(message.AuthorName, fonts.Metadata, "Author");
        var local = message.SentAtUtc.ToLocalTime();
        _time = CreateLabel(local.ToString("HH:mm:ss"), fonts.Metadata, "Time");
        _time.AccessibleDescription = local.ToString("yyyy-MM-dd HH:mm:ss zzz");
        _body = CreateLabel(message.Text, fonts.Body, "Body");
        Controls.AddRange([_author, _time, _body]);
        ChatTheme.Apply(this);
    }

    private static Label CreateLabel(string text, Font font, string name) => new()
    {
        Name = name,
        Text = text,
        Font = font,
        UseMnemonic = false,
        AutoSize = false,
        AccessibleName = name,
        AccessibleDescription = text,
        TabStop = false
    };

    internal void FitWidth(int width)
    {
        width = Math.Max(1, width);
        if (_measuredWidth == width && _measuredDpi == DeviceDpi) { return; }
        _measuredWidth = width;
        _measuredDpi = DeviceDpi;
        Width = width;
        var inset = (int)Math.Round(12 * DeviceDpi / 96F);
        var gap = (int)Math.Round(6 * DeviceDpi / 96F);
        var available = Math.Max(1, Width - inset * 2);
        var metadataHeight = TextRenderer.MeasureText("Ag", _author.Font).Height;
        var timeWidth = TextRenderer.MeasureText(_time.Text, _time.Font).Width;
        _author.SetBounds(inset, inset, Math.Max(1, available - timeWidth - gap), metadataHeight);
        _time.SetBounds(Width - inset - timeWidth, inset, timeWidth, metadataHeight);
        var bodyHeight = TextRenderer.MeasureText(_body.Text, _body.Font, new Size(available, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPrefix).Height;
        _body.SetBounds(inset, inset + metadataHeight + gap, available, bodyHeight);
        Height = inset * 2 + metadataHeight + gap + bodyHeight;
    }

    internal void ApplyTheme()
    {
        BackColor = SystemInformation.HighContrast ? SystemColors.Window
            : Outgoing ? ChatTheme.Information : ChatTheme.Incoming;
        ForeColor = SystemInformation.HighContrast ? SystemColors.WindowText : ChatTheme.TextPrimary;
    }
}
