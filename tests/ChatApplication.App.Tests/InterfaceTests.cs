using ChatApplication.Client.Forms;
using ChatApplication.Presentation;
using ChatApplication.Server.Forms;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ChatApplication.App.Tests;

public sealed class InterfaceTests
{
    [Theory]
    [InlineData(false, 1120, 720)]
    [InlineData(true, 1120, 720)]
    [InlineData(false, 900, 620)]
    [InlineData(true, 900, 620)]
    [InlineData(false, 1440, 900)]
    [InlineData(true, 1440, 900)]
    public void Resizing_preserves_nonoverlapping_reachable_content(bool server, int width, int height) =>
        StaWindow.Run(() =>
        {
            using var form = CreateForm(server);
            form.Show();
            form.ApplyWorkingAreaBounds(new Rectangle(0, 0, 2000, 1400));
            form.ClientSize = LogicalSize(form, width, height);
            Application.DoEvents();
            var viewport = Assert.IsType<Panel>(form.Controls[0]);
            var geometry = $"Client={form.ClientSize}; DPI={form.DeviceDpi}; viewport={viewport.ClientSize}; virtual={viewport.AutoScrollMinSize}; content={viewport.Controls[0].Size}";
            Assert.False(viewport.HorizontalScroll.Visible, "Unnecessary horizontal scrollbar. " + geometry);
            Assert.False(viewport.VerticalScroll.Visible, "Unnecessary vertical scrollbar. " + geometry);
            AssertLayout(form);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Small_working_area_keeps_window_inside_and_content_scrollable(bool server) =>
        StaWindow.Run(() =>
        {
            using var form = CreateForm(server);
            form.Show();
            var area = new Rectangle(50, 60, 640, 480);
            form.ApplyWorkingAreaBounds(area);
            Application.DoEvents();
            Assert.True(area.Contains(form.Bounds), $"Window {form.Bounds} escaped {area}.");
            var viewport = Assert.IsType<Panel>(form.Controls[0]);
            Assert.True(viewport.AutoScroll);
            Assert.True(viewport.DisplayRectangle.Width > viewport.ClientSize.Width);
            Assert.True(viewport.DisplayRectangle.Height > viewport.ClientSize.Height);
            viewport.AutoScrollPosition = new Point(int.MaxValue, int.MaxValue);
            Application.DoEvents();
            Assert.True(viewport.AutoScrollPosition.X < 0);
            Assert.True(viewport.AutoScrollPosition.Y < 0);
            AssertLayout(form);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Editing_settings_and_unavailable_send_cannot_create_chat_data(bool server) =>
        StaWindow.Run(() =>
        {
            using var form = CreateForm(server);
            form.Show();
            var transcript = Descendants(form).Single(control => control.Name == "Transcript");
            var before = Descendants(transcript).OfType<Label>().Select(label => label.Text).ToArray();
            foreach (var input in Descendants(form).OfType<TextBox>().Where(input => input.Enabled && input.Parent is not UpDownBase))
            {
                input.Text = "Test & input — مرحبا — Türkçe";
            }

            foreach (var button in Descendants(form).OfType<Button>().Where(button => button.Name == "SendMessage"))
            {
                Assert.False(button.Enabled);
                button.PerformClick();
            }

            Application.DoEvents();
            Assert.Equal(before, Descendants(transcript).OfType<Label>().Select(label => label.Text).ToArray());
            Assert.All(Descendants(form).OfType<ListView>(), list => Assert.Empty(list.Items));
            Assert.All(Descendants(form).OfType<TextBox>().Where(input => !input.Enabled),
                input => Assert.Empty(input.Text));
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Tab_navigation_reaches_editable_settings_and_available_actions(bool server) =>
        StaWindow.Run(() =>
        {
            using var form = CreateForm(server);
            form.Show();
            var settings = Descendants(form)
                .Where(control => control.Enabled && control.TabStop && control.Parent is not UpDownBase
                    && (control is TextBox || control is NumericUpDown || control is Button))
                .ToArray();
            var reached = new HashSet<Control>();
            for (var step = 0; step < settings.Length * 2; step++)
            {
                Assert.True(form.SelectNextControl(form.ActiveControl, true, true, true, true));
                var active = form.ActiveControl;
                Assert.NotNull(active);
                Assert.Contains(active, settings);
                reached.Add(active);
            }

            Assert.All(settings, setting => Assert.Contains(setting, reached));
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Long_guidance_reflows_without_hiding_essential_text(bool server) =>
        StaWindow.Run(() =>
        {
            using var form = CreateForm(server);
            form.Show();
            form.ApplyWorkingAreaBounds(new Rectangle(0, 0, 2000, 1400));
            form.ClientSize = LogicalSize(form, 900, 620);
            var feedback = Descendants(form).OfType<Label>().Single(label => label.Name == "Feedback");
            feedback.Text = string.Join(" ", Enumerable.Repeat(
                "Long local guidance & literal text must remain readable without overlapping the composer.", 4));
            Application.DoEvents();
            AssertLayout(form);
        });

    [Fact]
    public void Closing_one_window_does_not_invalidate_the_other_windows_fonts_or_painting() =>
        StaWindow.Run(() =>
        {
            using var client = new ClientForm();
            using (var server = new ServerForm())
            {
                client.Show();
                server.Show();
                Application.DoEvents();
                server.Close();
            }

            client.ClientSize = LogicalSize(client, 1000, 680);
            Application.DoEvents();
            using var bitmap = new Bitmap(client.Width, client.Height);
            client.DrawToBitmap(bitmap, client.ClientRectangle);
            foreach (var label in Descendants(client).OfType<Label>())
            {
                Assert.True(TextRenderer.MeasureText(label.Text, label.Font).Height > 0);
            }

            Assert.False(client.IsDisposed);
            AssertLayout(client);
        });

    [Fact]
    public void Actual_forms_complete_handshake_reconnect_and_close_with_empty_send_disabled() =>
        StaWindow.Run(() =>
        {
            using var reservation = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            reservation.Start();
            var port = ((System.Net.IPEndPoint)reservation.LocalEndpoint).Port;
            reservation.Stop();
            using var server = new ServerForm();
            using var client = new ClientForm();
            server.Show();
            client.Show();
            Find<NumericUpDown>(server, "Port").Value = port;
            Find<NumericUpDown>(client, "Port").Value = port;
            Find<TextBox>(client, "Username").Text = "علاء";
            Find<Button>(server, "StartServer").PerformClick();
            StaWindow.Until(() => Badge(server) == "Listening");
            for (var cycle = 0; cycle < 2; cycle++)
            {
                Find<Button>(client, "Connect").PerformClick();
                StaWindow.Until(() => Badge(client) == "Connected"
                    && Find<Label>(server, "PeerSummary").Text.Contains("علاء"));
                Assert.False(Find<TextBox>(client, "Username").Enabled);
                Assert.False(Find<Button>(client, "SendMessage").Enabled);
                Assert.Empty(Find<TextBox>(client, "MessageInput").Text);
                Assert.Equal("علاء", Assert.Single(Find<ListView>(server, "ConnectedClients").Items.Cast<ListViewItem>()).Text);
                if (cycle == 0)
                {
                    Find<Button>(client, "Disconnect").PerformClick();
                    StaWindow.Until(() => Badge(client) == "Disconnected"
                        && Find<Label>(server, "PeerSummary").Text.StartsWith("No accepted peer."));
                }
            }
            client.Close();
            StaWindow.Until(() => client.IsDisposed
                && Find<Label>(server, "PeerSummary").Text.StartsWith("No accepted peer."));
            Find<Button>(server, "StopServer").PerformClick();
            StaWindow.Until(() => Badge(server) == "Stopped");
            server.Close();
            StaWindow.Until(() => server.IsDisposed);
        });

    [Fact]
    public void Occupied_port_feedback_keeps_server_controls_recoverable() =>
        StaWindow.Run(() =>
        {
            using var occupied = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            occupied.Start();
            using var server = new ServerForm();
            server.Show();
            Find<NumericUpDown>(server, "Port").Value = ((System.Net.IPEndPoint)occupied.LocalEndpoint).Port;
            Find<Button>(server, "StartServer").PerformClick();
            StaWindow.Until(() => Badge(server) == "Stopped"
                && Find<Label>(server, "Feedback").Text.Contains("already in use"));
            Assert.True(Find<Button>(server, "StartServer").Enabled);
            Assert.True(Find<TextBox>(server, "BindAddress").Enabled);
            server.Close();
            StaWindow.Until(() => server.IsDisposed);
        });

    private static T Find<T>(Control root, string name) where T : Control =>
        Descendants(root).OfType<T>().Single(control => control.Name == name);

    private static string Badge(Control root) => Descendants(root)
        .OfType<ChatApplication.Presentation.Controls.ConnectionBadge>().Single().Text;

    private static Size LogicalSize(Form form, int width, int height) =>
        new((int)Math.Round(width * form.DeviceDpi / 96F), (int)Math.Round(height * form.DeviceDpi / 96F));

    private static ChatWindow CreateForm(bool server) => server ? new ServerForm() : new ClientForm();

    private static IEnumerable<Control> Descendants(Control root) => StaWindow.Descendants(root);

    internal static void AssertLayout(Form form)
    {
        foreach (var table in Descendants(form).OfType<TableLayoutPanel>())
        {
            var children = table.Controls.Cast<Control>().Where(control => control.Visible).ToArray();
            foreach (var child in children)
            {
                Assert.True(child.Width > 0 && child.Height > 0, $"{child.Name}/{child.GetType().Name} collapsed.");
                Assert.True(table.DisplayRectangle.Contains(child.Bounds),
                    $"{child.Name}/{child.Text} {child.Bounds} escaped {table.Name}/{table.GetType().Name} {table.DisplayRectangle}.");
                if (child is Label label)
                {
                    var preferred = label.GetPreferredSize(new Size(label.Width, 0));
                    Assert.True(label.Height >= preferred.Height,
                        $"Clipped label: {label.Text}, actual {label.Size}, preferred {preferred}.");
                }
            }

            for (var first = 0; first < children.Length; first++)
            {
                for (var second = first + 1; second < children.Length; second++)
                {
                    Assert.False(children[first].Bounds.IntersectsWith(children[second].Bounds),
                        $"Overlapping controls in {table.Name}: {children[first].Text} / {children[second].Text}.");
                }
            }
        }
    }
}
