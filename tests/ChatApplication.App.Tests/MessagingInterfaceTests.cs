using System.Reflection;
using ChatApplication.Client.Forms;
using ChatApplication.Core;
using ChatApplication.Presentation;
using ChatApplication.Presentation.Controls;
using ChatApplication.Presentation.Theme;
using ChatApplication.Server.Forms;
using Xunit;

namespace ChatApplication.App.Tests;

public sealed class MessagingInterfaceTests
{
    private static ChatMessage Message(string text, MessageAuthorRole role = MessageAuthorRole.Server) =>
        new(Guid.NewGuid(), role, role == MessageAuthorRole.Server ? "Server" : "Alaa", text, DateTimeOffset.UnixEpoch);

    [Fact]
    public void Transcript_renders_literal_metadata_body_and_wraps_without_ordering_by_time() =>
        StaWindow.Run(() =>
        {
            using var fonts = new OwnedFonts();
            using var form = new Form { ClientSize = new Size(650, 500) };
            using var view = new TranscriptView(fonts);
            form.Controls.Add(view);
            form.Show();
            var message = Message("Literal & <script> 😀 مرحبا Türkçe\r\n" + new string('x', 180));
            view.Append(message, true);
            view.Append(message, true);
            var older = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Client, "Alaa", "arrived second",
                DateTimeOffset.UnixEpoch.AddSeconds(-1));
            view.Append(older, false);
            Application.DoEvents();
            var cards = StaWindow.Descendants(view).OfType<MessageCard>().ToArray();
            Assert.Equal(2, view.MessageCount);
            Assert.Equal(message, cards[0].Message);
            Assert.Equal(older, cards[1].Message);
            var body = Find<Label>(cards[0], "Body");
            Assert.Equal(message.Text, body.Text);
            Assert.False(body.UseMnemonic);
            Assert.Equal("Server", Find<Label>(cards[0], "Author").Text);
            Assert.Equal(message.SentAtUtc.ToLocalTime().ToString("HH:mm:ss"), Find<Label>(cards[0], "Time").Text);
            Assert.Equal(message.SentAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz"),
                Find<Label>(cards[0], "Time").AccessibleDescription);
            var originalHeight = cards[0].Height;
            form.ClientSize = new Size(320, 500);
            Application.DoEvents();
            Assert.True(cards[0].Height > originalHeight);
            Assert.True(cards[0].ClientRectangle.Contains(body.Bounds));
            Assert.True(cards[1].Top >= cards[0].Bottom);
        });

    [Fact]
    public void Retention_cap_disposes_old_cards_keeps_latest_ids_and_persistent_notice() =>
        StaWindow.Run(() =>
        {
            using var fonts = new OwnedFonts();
            using var form = new Form { ClientSize = new Size(600, 500) };
            using var view = new TranscriptView(fonts);
            form.Controls.Add(view);
            view.Append(Message("first"), false);
            for (var i = 1; i < 500; i++) { view.Append(Message($"message {i}"), false); }
            form.Show();
            var first = StaWindow.Descendants(view).OfType<MessageCard>().First();
            Assert.True(first.IsHandleCreated);
            view.Append(Message("message 500"), false);
            view.Append(Message("message 501"), false);
            Assert.Equal(500, view.MessageCount);
            Assert.True(first.IsDisposed);
            Assert.True(view.HasTrimmedMessages);
            Assert.True(Find<Label>(view, "TrimNotice").Visible);
            var cards = StaWindow.Descendants(view).OfType<MessageCard>().ToArray();
            Assert.Equal(500, cards.Length);
            Assert.Equal("message 2", cards[0].Message.Text);
            Assert.Equal("message 501", cards[^1].Message.Text);
            Assert.False(Find<Label>(view, "EmptyConversation").Visible);
            view.Dispose();
            Assert.All(cards, card => Assert.True(card.IsDisposed));
        });

    [Fact]
    public void Composer_remains_unavailable_before_connection_and_rejects_whitespace_and_over_limit_text() =>
        StaWindow.Run(() =>
        {
            using var form = new ProbeWindow();
            form.Show();
            var input = Find<MessageComposer>(form, "MessageInput");
            var send = Find<Button>(form, "SendMessage");
            Assert.False(input.Enabled);
            form.Available(true);
            input.Text = " \r\n";
            Assert.False(send.Enabled);
            input.Text = new string('x', 2001);
            Assert.False(send.Enabled);
            Assert.Contains("-1 remaining", StaWindow.Descendants(form).OfType<Label>().Single(label => label.Text.Contains("remaining")).Text);
        });

    [Fact]
    public void Send_button_and_synthetic_enter_share_confirmation_and_failure_lifecycle() =>
        StaWindow.Run(() =>
        {
            using var form = new ProbeWindow();
            form.Show();
            form.Available(true);
            var input = Find<MessageComposer>(form, "MessageInput");
            var send = Find<Button>(form, "SendMessage");
            input.Text = "first";
            Enter(input);
            Enter(input);
            send.PerformClick();
            Assert.Equal(1, form.Requests);
            Assert.False(input.Enabled);
            Assert.Equal(0, Find<TranscriptView>(form, "Transcript").MessageCount);
            form.Deliver(Message("unrelated"));
            Assert.Equal("first", input.Text);
            input.Text = "newer programmatic text";
            form.Confirm("first");
            StaWindow.Until(() => input.Enabled);
            Assert.Equal("newer programmatic text", input.Text);
            send.PerformClick();
            Assert.Equal(2, form.Requests);
            form.Confirm(input.Text);
            StaWindow.Until(() => input.Enabled);
            Assert.Empty(input.Text);
            Assert.False(send.Enabled);
            input.Text = "retain on failure";
            send.PerformClick();
            form.Available(false);
            form.Fail();
            StaWindow.Until(() => Find<Label>(form, "Feedback").Text.Contains("not confirmed"));
            Assert.Equal("retain on failure", input.Text);
            Assert.False(input.Enabled);
            form.Close();
            StaWindow.Until(() => form.IsDisposed);
        });

    [Fact]
    public void Closing_while_send_pending_cancels_owned_work_and_disposes_window() =>
        StaWindow.Run(() =>
        {
            using var form = new ProbeWindow();
            form.Show();
            form.Available(true);
            Find<MessageComposer>(form, "MessageInput").Text = "pending";
            Find<Button>(form, "SendMessage").PerformClick();
            Assert.Equal(1, form.Requests);
            form.Close();
            StaWindow.Until(() => form.IsDisposed);
            Assert.True(form.Pending.IsCompleted);
        });

    [Fact]
    public void Real_forms_exchange_once_both_directions_and_keep_draft_when_server_stops() =>
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
            Find<TextBox>(client, "Username").Text = "Alaa";
            Find<Button>(server, "StartServer").PerformClick();
            Find<Button>(client, "Connect").PerformClick();
            StaWindow.Until(() => Find<MessageComposer>(server, "MessageInput").Enabled
                && Find<MessageComposer>(client, "MessageInput").Enabled);
            var clientInput = Find<MessageComposer>(client, "MessageInput");
            var serverInput = Find<MessageComposer>(server, "MessageInput");
            foreach (var text in new[] { "مرحبا & Türkçe 👋\r\nline 2", "repeat", "repeat" })
            {
                clientInput.Text = text;
                Find<Button>(client, "SendMessage").PerformClick();
                StaWindow.Until(() => clientInput.Enabled && clientInput.Text.Length == 0);
            }
            serverInput.Text = "Server reply";
            Find<Button>(server, "SendMessage").PerformClick();
            StaWindow.Until(() => serverInput.Enabled && serverInput.Text.Length == 0
                && Find<TranscriptView>(client, "Transcript").MessageCount == 4);
            var serverMessages = StaWindow.Descendants(server).OfType<MessageCard>().Select(card => card.Message).ToArray();
            var clientMessages = StaWindow.Descendants(client).OfType<MessageCard>().Select(card => card.Message).ToArray();
            Assert.Equal(serverMessages, clientMessages);
            Assert.Equal(4, serverMessages.Select(message => message.MessageId).Distinct().Count());
            clientInput.Text = "unsent draft";
            Find<Button>(server, "StopServer").PerformClick();
            StaWindow.Until(() => !clientInput.Enabled);
            Assert.Equal("unsent draft", clientInput.Text);
            client.Close();
            server.Close();
            StaWindow.Until(() => client.IsDisposed && server.IsDisposed);
        });

    private static T Find<T>(Control root, string name) where T : Control =>
        StaWindow.Descendants(root).OfType<T>().Single(control => control.Name == name);

    private static void Enter(MessageComposer input)
    {
        var method = typeof(MessageComposer).GetMethod("ProcessCmdKey", BindingFlags.Instance | BindingFlags.NonPublic)!;
        Assert.True((bool)method.Invoke(input, [System.Windows.Forms.Message.Create(input.Handle, 0x0100, (nint)13, 0), Keys.Enter])!);
    }

    private sealed class ProbeWindow : ChatWindow
    {
        private TaskCompletionSource<ChatMessage> _pending = NewPending();
        public int Requests { get; private set; }
        public Task Pending => _pending.Task;
        public ProbeWindow() : base("Client") { CompleteLayout(); }
        public void Available(bool available) => SetMessagingAvailable(available);
        public void Deliver(ChatMessage message) => AppendMessage(message);
        public void Confirm(string text)
        {
            var message = Message(text, MessageAuthorRole.Client);
            Deliver(message);
            _pending.TrySetResult(message);
        }
        public void Fail() => _pending.TrySetException(new IOException("Synthetic failure."));
        protected override Task<ChatMessage> SendMessageAsync(string text)
        {
            Requests++;
            _pending = NewPending();
            return _pending.Task;
        }
        protected override Task CloseConnectionAsync()
        {
            _pending.TrySetCanceled();
            return Task.CompletedTask;
        }
        private static TaskCompletionSource<ChatMessage> NewPending() => new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
