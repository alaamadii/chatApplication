using ChatApplication.Core;
using ChatApplication.Presentation.Controls;
using ChatApplication.Presentation.Theme;
using Xunit;

namespace ChatApplication.App.Tests;

public sealed class RobustnessInterfaceTests
{
    [Fact]
    public void Visible_burst_trims_live_controls_and_remains_resizable_and_closable() =>
        StaWindow.Run(() =>
        {
            using var fonts = new OwnedFonts();
            using var form = new Form { ClientSize = new Size(650, 500) };
            using var view = new TranscriptView(fonts);
            form.Controls.Add(view);
            form.Show();
            MessageCard? first = null;
            for (var index = 0; index < 512; index++)
            {
                view.Append(new(Guid.NewGuid(), MessageAuthorRole.Server, "Server",
                    $"message {index} & Türkçe مرحبا 👋\nsecond line", DateTimeOffset.UnixEpoch), index % 2 == 0);
                if (index == 0) { first = StaWindow.Descendants(view).OfType<MessageCard>().Single(); }
                if (index % 16 == 0) { Application.DoEvents(); }
            }
            Application.DoEvents();
            Assert.Equal(500, view.MessageCount);
            Assert.True(first!.IsDisposed);
            Assert.True(view.HasTrimmedMessages);
            var latest = StaWindow.Descendants(view).OfType<MessageCard>().Last();
            Assert.True(view.RectangleToScreen(view.ClientRectangle).IntersectsWith(latest.RectangleToScreen(latest.ClientRectangle)));
            form.ClientSize = new Size(450, 400);
            Application.DoEvents();
            var cards = StaWindow.Descendants(view).OfType<MessageCard>().ToArray();
            Assert.Equal(500, cards.Length);
            Assert.Equal("message 12 & Türkçe مرحبا 👋\nsecond line", cards[0].Message.Text);
            Assert.Contains("message 511", cards[^1].Message.Text);
            Assert.All(cards, card =>
            {
                Assert.True(card.Width > 0 && card.Height > 0);
                Assert.All(card.Controls.Cast<Control>(), label => Assert.True(card.ClientRectangle.Contains(label.Bounds)));
            });
            form.Close();
            Assert.True(form.IsDisposed);
            Assert.All(cards, card => Assert.True(card.IsDisposed));
        });

    [Fact]
    public void Server_list_tracks_only_accepted_metadata_and_clears_after_disconnect() =>
        StaWindow.Run(() =>
        {
            using var reservation = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            reservation.Start();
            var port = ((System.Net.IPEndPoint)reservation.LocalEndpoint).Port;
            reservation.Stop();
            using var server = new ChatApplication.Server.Forms.ServerForm();
            server.Show();
            Find<NumericUpDown>(server, "Port").Value = port;
            Find<Button>(server, "StartServer").PerformClick();
            var list = Find<ListView>(server, "ConnectedClients");
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("ar-SA");
            var before = DateTimeOffset.UtcNow;
            using var socket = new System.Net.Sockets.TcpClient();
            Complete(socket.ConnectAsync(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port)));
            var actualEndpoint = (System.Net.IPEndPoint)socket.Client.LocalEndPoint!;
            using var wire = new ChatApplication.Networking.Protocol.FramedConnection(socket.GetStream());
            Assert.Empty(list.Items);
            Complete(wire.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.Hello("Alaa")));
            var welcomeTask = wire.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ClientHandshake);
            Complete(welcomeTask);
            StaWindow.Until(() => list.Items.Count == 1);
            var row = list.Items[0];
            var info = Assert.IsType<ChatApplication.Networking.ConnectedClientInfo>(row.Tag);
            Assert.Equal(welcomeTask.GetAwaiter().GetResult()!.SessionId, info.SessionId);
            Assert.Equal("Alaa", row.Text);
            Assert.Equal(new System.Net.IPEndPoint(actualEndpoint.Address.MapToIPv4(), actualEndpoint.Port).ToString(), row.SubItems[1].Text);
            Assert.Equal(info.ConnectedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture), row.SubItems[2].Text);
            Assert.Equal("Connected", row.SubItems[3].Text);
            Assert.InRange(info.ConnectedAtUtc, before, DateTimeOffset.UtcNow);
            Assert.Contains(row.SubItems[2].Text, list.AccessibleDescription);
            Assert.True(list.TabStop);
            using var busy = new System.Net.Sockets.TcpClient();
            Complete(busy.ConnectAsync(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port)));
            using var rejection = new ChatApplication.Networking.Protocol.FramedConnection(busy.GetStream());
            Complete(rejection.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.Hello("Other")));
            var error = rejection.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ClientHandshake);
            Complete(error);
            Assert.Equal("server_busy", error.GetAwaiter().GetResult()!.Code);
            Assert.Same(row, Assert.Single(list.Items.Cast<ListViewItem>()));
            foreach (var width in new[] { 900, 1440 })
            {
                server.ClientSize = new Size((int)Math.Round(width * server.DeviceDpi / 96F), (int)Math.Round(720 * server.DeviceDpi / 96F));
                Application.DoEvents();
                InterfaceTests.AssertLayout(server);
                Assert.True(list.Columns.Cast<ColumnHeader>().Sum(column => column.Width) > list.ClientSize.Width);
                using var graphics = list.CreateGraphics();
                for (var column = 0; column < 4; column++)
                {
                    Assert.True(list.Columns[column].Width >= TextRenderer.MeasureText(graphics, row.SubItems[column].Text, list.Font).Width);
                }
            }
            Complete(wire.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.Disconnect()));
            StaWindow.Until(() => list.Items.Count == 0);
            Assert.False(list.TabStop);
            Assert.Contains("No accepted client", list.AccessibleDescription);
            server.Close();
            StaWindow.Until(() => server.IsDisposed);
        });

    [Fact]
    public void Failed_send_guidance_survives_state_edits_and_clears_only_after_confirmed_retry() =>
        StaWindow.Run(() =>
        {
            using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            using var client = new ChatApplication.Client.Forms.ClientForm();
            client.Show();
            var username = Find<TextBox>(client, "Username");
            username.Text = "First";
            Find<NumericUpDown>(client, "Port").Value = port;
            var accept = listener.AcceptTcpClientAsync();
            Find<Button>(client, "Connect").PerformClick();
            Assert.False(username.Enabled); // Lock immediately, including old-session preparation.
            Complete(accept);
            using var socket = accept.GetAwaiter().GetResult();
            using var wire = new ChatApplication.Networking.Protocol.FramedConnection(socket.GetStream());
            Complete(wire.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ServerHandshake));
            Complete(wire.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.Welcome("First", Guid.NewGuid())));
            var input = Find<MessageComposer>(client, "MessageInput");
            StaWindow.Until(() => input.Enabled);
            input.Text = "retained draft";
            Find<Button>(client, "SendMessage").PerformClick();
            Complete(wire.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ServerAccepted));
            socket.Client.LingerState = new(true, 0);
            socket.Dispose();
            var feedback = Find<Label>(client, "Feedback");
            StaWindow.Until(() => feedback.Text.Contains("not confirmed") && username.Enabled);
            username.Text = "Second"; // Causes a new connection-state render.
            Assert.Contains("not confirmed", feedback.Text);
            Assert.Equal("retained draft", input.Text);
            var secondAccept = listener.AcceptTcpClientAsync();
            var reconnect = Find<Button>(client, "Connect");
            StaWindow.Until(() => reconnect.Enabled);
            Assert.Contains("not confirmed", feedback.Text);
            reconnect.PerformClick();
            Complete(secondAccept);
            using var second = secondAccept.GetAwaiter().GetResult();
            using var secondWire = new ChatApplication.Networking.Protocol.FramedConnection(second.GetStream());
            Complete(secondWire.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ServerHandshake));
            Complete(secondWire.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.Welcome("Second", Guid.NewGuid())));
            StaWindow.Until(() => input.Enabled);
            Assert.Contains("not confirmed", feedback.Text);
            Find<Button>(client, "SendMessage").PerformClick();
            var request = secondWire.ReadAsync(ChatApplication.Networking.Protocol.PacketPhase.ServerAccepted);
            Complete(request);
            Assert.Equal("retained draft", request.GetAwaiter().GetResult()!.Text);
            Complete(secondWire.WriteAsync(ChatApplication.Networking.Protocol.ConnectionPacket.AcceptedMessage(
                new(Guid.NewGuid(), MessageAuthorRole.Client, "Second", "retained draft", DateTimeOffset.UtcNow))));
            StaWindow.Until(() => input.Enabled && input.Text.Length == 0);
            Assert.DoesNotContain("not confirmed", feedback.Text);
            client.Close();
            StaWindow.Until(() => client.IsDisposed);
        });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Closing_either_real_form_during_submission_releases_the_owned_port(bool closeServer) =>
        StaWindow.Run(() =>
        {
            using var reservation = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            reservation.Start();
            var port = ((System.Net.IPEndPoint)reservation.LocalEndpoint).Port;
            reservation.Stop();
            using var server = new ChatApplication.Server.Forms.ServerForm();
            using var client = new ChatApplication.Client.Forms.ClientForm();
            server.Show();
            client.Show();
            Find<NumericUpDown>(server, "Port").Value = port;
            Find<NumericUpDown>(client, "Port").Value = port;
            Find<TextBox>(client, "Username").Text = "Alaa";
            Find<Button>(server, "StartServer").PerformClick();
            Find<Button>(client, "Connect").PerformClick();
            StaWindow.Until(() => Find<MessageComposer>(server, "MessageInput").Enabled
                && Find<MessageComposer>(client, "MessageInput").Enabled);
            Form closing = closeServer ? server : client;
            Form remaining = closeServer ? client : server;
            Find<MessageComposer>(closing, "MessageInput").Text = "close during submission";
            Find<Button>(closing, "SendMessage").PerformClick();
            closing.Close();
            StaWindow.Until(() => closing.IsDisposed && !Find<MessageComposer>(remaining, "MessageInput").Enabled);
            if (!closeServer) { Assert.Empty(Find<ListView>(remaining, "ConnectedClients").Items); }
            remaining.Close();
            StaWindow.Until(() => remaining.IsDisposed);
            using var rebind = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
            rebind.Start();
        });

    private static T Find<T>(Control root, string name) where T : Control =>
        StaWindow.Descendants(root).OfType<T>().Single(control => control.Name == name);

    // Observe completion after pumping; never block the UI thread on unfinished network I/O.
    private static void Complete(Task task)
    {
        StaWindow.Until(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
    }
}
