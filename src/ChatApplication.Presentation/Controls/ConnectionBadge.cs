using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Controls;

public sealed class ConnectionBadge : Label
{
    public ConnectionBadge()
    {
        Text = "Interface preview";
        AutoSize = true;
        Padding = new Padding(12, 8, 12, 8);
        UseMnemonic = false;
        AccessibleName = "Implementation status";
        AccessibleDescription = "Interface preview. Networking and messaging are not implemented.";
    }

    internal void ApplyTheme()
    {
        BackColor = ChatTheme.Information;
        ForeColor = ChatTheme.TextPrimary;
    }
}
