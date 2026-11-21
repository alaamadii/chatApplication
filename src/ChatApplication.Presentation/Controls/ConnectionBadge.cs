using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Controls;

public enum ConnectionTone { Neutral, Success, Warning, Error }

public sealed class ConnectionBadge : Label
{
    private ConnectionTone _tone;

    public ConnectionBadge()
    {
        AutoSize = true;
        Padding = new Padding(12, 8, 12, 8);
        UseMnemonic = false;
        AccessibleName = "Connection status";
    }

    public void SetStatus(string text, ConnectionTone tone)
    {
        Text = text;
        AccessibleDescription = text;
        _tone = tone;
        ApplyTheme();
    }

    internal void ApplyTheme()
    {
        (ForeColor, BackColor) = SystemInformation.HighContrast
            ? (SystemColors.ControlText, SystemColors.Control)
            : _tone switch
            {
                ConnectionTone.Success => (Color.FromArgb(22, 101, 52), Color.FromArgb(220, 252, 231)),
                ConnectionTone.Warning => (Color.FromArgb(146, 64, 14), Color.FromArgb(254, 243, 199)),
                ConnectionTone.Error => (Color.FromArgb(153, 27, 27), Color.FromArgb(254, 226, 226)),
                _ => (ChatTheme.TextPrimary, ChatTheme.Information)
            };
    }
}
