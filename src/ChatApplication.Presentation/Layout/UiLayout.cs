using ChatApplication.Presentation.Theme;

namespace ChatApplication.Presentation.Layout;

public static class UiLayout
{
    public static TableLayoutPanel Column(Padding padding = default) => new()
    {
        ColumnCount = 1,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Top,
        Padding = padding,
        Margin = Padding.Empty,
        ColumnStyles = { new ColumnStyle(SizeType.Percent, 100F) }
    };

    public static void AddRow(TableLayoutPanel column, Control child)
    {
        var row = column.RowCount++;
        column.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        child.TabIndex = row;
        column.Controls.Add(child, 0, row);
    }

    public static Label Text(string text, Font font, bool secondary = false) => new()
    {
        Text = text,
        Font = font,
        AutoSize = true,
        Dock = DockStyle.Fill,
        Margin = new Padding(0, 0, 0, 12),
        UseMnemonic = false,
        Tag = secondary ? ThemeRole.SecondaryText : null
    };

    public static Control Field(string caption, Control input)
    {
        var column = Column();
        var label = Text(caption, input.Font);
        label.UseMnemonic = true;
        label.Margin = new Padding(0, 0, 0, 8);
        input.Dock = DockStyle.Top;
        input.Margin = new Padding(0, 0, 0, 12);
        AddRow(column, label);
        AddRow(column, input);
        return column;
    }

    public static Button ActionButton(string text, string name) => new()
    {
        Text = text,
        Name = name,
        AccessibleName = text.Replace("&", ""),
        AccessibleDescription = "Availability follows connection state and input validation.",
        Enabled = false,
        Dock = DockStyle.Fill,
        Height = 36,
        FlatStyle = FlatStyle.System,
        Margin = new Padding(0, 0, 8, 8)
    };

    public static Control Actions(Button first, Button second)
    {
        var row = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = Padding.Empty,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 50F),
                new ColumnStyle(SizeType.Percent, 50F)
            },
            RowStyles = { new RowStyle(SizeType.AutoSize) }
        };
        second.Margin = new Padding(0, 0, 0, 8);
        first.TabIndex = 0;
        second.TabIndex = 1;
        row.Controls.Add(first, 0, 0);
        row.Controls.Add(second, 1, 0);
        return row;
    }
}
