using ChatApplication.Presentation;
using ChatApplication.Presentation.Layout;
using ChatApplication.Networking;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm : ChatWindow
{
    private readonly TextBox _bindAddress = new()
    {
        Name = "BindAddress",
        AccessibleName = "Bind IPv4 address",
        Text = "127.0.0.1"
    };
    private readonly NumericUpDown _port = new()
    {
        Name = "Port",
        AccessibleName = "Listener port",
        Minimum = 1,
        Maximum = 65535,
        Value = 5000
    };
    private readonly ListView _clients = new()
    {
        Name = "ConnectedClients",
        AccessibleName = "Connected client information",
        AccessibleDescription = "No accepted client. Pending and rejected connections are not listed.",
        View = View.Details,
        Height = 96,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 0, 0, 12),
        TabStop = false,
        FullRowSelect = true,
        HideSelection = false,
        ShowItemToolTips = true
    };

    private readonly Button _startButton = UiLayout.ActionButton("S&tart server", "StartServer");
    private readonly Button _stopButton = UiLayout.ActionButton("St&op server", "StopServer");
    private Label _peerSummary = null!;

    public ServerForm() : base("Server")
    {
        _server = new(OnConnectionAsync, onMessage: OnMessageAsync);
        BuildLayout();
        CompleteLayout();
        _startButton.Click += StartClicked;
        _stopButton.Click += StopClicked;
        _bindAddress.TextChanged += (_, _) => RenderConnection(_server.Snapshot);
        _port.ValueChanged += (_, _) => RenderConnection(_server.Snapshot);
        _clients.HandleCreated += (_, _) => FitClientColumns();
        _clients.DpiChangedAfterParent += (_, _) => FitClientColumns();
        RenderConnection(_server.Snapshot);
    }
}
