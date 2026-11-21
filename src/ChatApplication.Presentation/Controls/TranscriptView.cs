using ChatApplication.Presentation.Layout;
using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Controls;

public sealed class TranscriptView : UserControl
{
    public TranscriptView(OwnedFonts fonts)
    {
        Name = "Transcript";
        AccessibleName = "Empty conversation";
        AccessibleDescription = "No messages. Sending and receiving arrive in P03.";
        Dock = DockStyle.Fill;
        MinimumSize = new Size(0, 96);
        Margin = new Padding(0, 0, 0, 16);
        Tag = ThemeRole.Surface;
        TabStop = false;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            RowStyles =
            {
                new RowStyle(SizeType.Percent, 50F),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 50F)
            },
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100F) }
        };
        var heading = UiLayout.Text("Your conversation starts here", fonts.Section);
        heading.TextAlign = ContentAlignment.MiddleCenter;
        var help = UiLayout.Text(
            "No messages have been sent or received.\nMessaging will be available in P03.",
            fonts.Body, secondary: true);
        help.TextAlign = ContentAlignment.MiddleCenter;
        layout.Controls.Add(heading, 0, 1);
        layout.Controls.Add(help, 0, 2);
        Controls.Add(layout);
    }
}
