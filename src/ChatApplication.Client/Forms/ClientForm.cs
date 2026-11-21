namespace ChatApplication.Client.Forms;

internal sealed class ClientForm : Form
{
    public ClientForm()
    {
        Text = "Chat Application - Client";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(800, 480);
        MinimumSize = new Size(640, 400);
        StartPosition = FormStartPosition.CenterScreen;

        // Native controls inherit the system font; the form owns their lifetime.
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 4
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        layout.Controls.Add(CreateLabel("YMH219 Chat Application | Client"), 0, 0);
        layout.Controls.Add(CreateLabel("Phase P00 - project initialization"), 0, 1);
        layout.Controls.Add(CreateLabel(
            "Networking and messaging are not implemented yet. "
            + "The complete interface arrives in Phase P01."), 0, 2);

        Controls.Add(layout);
    }

    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Dock = DockStyle.Fill,
        Margin = new Padding(0, 0, 0, 16),
        UseMnemonic = false
    };
}
