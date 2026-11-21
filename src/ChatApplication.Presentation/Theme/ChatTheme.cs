using ChatApplication.Presentation.Controls;

namespace ChatApplication.Presentation.Theme;

public enum ThemeRole
{
    Canvas,
    Surface,
    Sidebar,
    SecondaryText
}

public static class ChatTheme
{
    public static Color Canvas => SystemInformation.HighContrast ? SystemColors.Control : Color.FromArgb(244, 246, 250);
    public static Color Surface => SystemInformation.HighContrast ? SystemColors.Window : Color.White;
    public static Color Sidebar => SystemInformation.HighContrast ? SystemColors.Control : Color.FromArgb(247, 249, 252);
    public static Color TextPrimary => SystemInformation.HighContrast ? SystemColors.ControlText : Color.FromArgb(23, 37, 61);
    public static Color TextSecondary => SystemInformation.HighContrast ? SystemColors.ControlText : Color.FromArgb(82, 97, 120);
    public static Color Incoming => SystemInformation.HighContrast ? SystemColors.Window : Color.FromArgb(241, 245, 249);
    public static Color Information => SystemInformation.HighContrast ? SystemColors.Control : Color.FromArgb(232, 238, 255);

    public static void Apply(Control root)
    {
        switch (root)
        {
            case MessageCard card:
                card.ApplyTheme();
                break;
            case ConnectionBadge badge:
                badge.ApplyTheme();
                break;
            case Label label:
                label.BackColor = Color.Transparent;
                label.ForeColor = SystemInformation.HighContrast
                    ? label.Parent?.ForeColor ?? SystemColors.ControlText
                    : label.Tag is ThemeRole.SecondaryText ? TextSecondary : TextPrimary;
                break;
            case Button button:
                button.UseVisualStyleBackColor = true;
                button.ForeColor = SystemColors.ControlText;
                break;
            case TextBox textBox:
                textBox.BackColor = textBox.Enabled ? SystemColors.Window : SystemColors.Control;
                textBox.ForeColor = SystemColors.WindowText;
                break;
            case NumericUpDown number:
                number.BackColor = SystemColors.Window;
                number.ForeColor = SystemColors.WindowText;
                break;
            case ListView list:
                list.BackColor = SystemColors.Window;
                list.ForeColor = SystemColors.WindowText;
                break;
            default:
                root.BackColor = root.Tag switch
                {
                    ThemeRole.Canvas => Canvas,
                    ThemeRole.Sidebar => Sidebar,
                    ThemeRole.Surface => Surface,
                    _ => root.Parent?.BackColor ?? Canvas
                };
                root.ForeColor = SystemInformation.HighContrast && root.BackColor == SystemColors.Window
                    ? SystemColors.WindowText : TextPrimary;
                break;
        }

        foreach (Control child in root.Controls)
        {
            Apply(child);
        }
    }
}
