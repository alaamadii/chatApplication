using ChatApplication.Presentation;
using ChatApplication.Presentation.Layout;
using ChatApplication.Networking;

namespace ChatApplication.Client.Forms;

internal sealed partial class ClientForm : ChatWindow
{
    private readonly TextBox _username = new()
    {
        Name = "Username",
        AccessibleName = "Username",
        AccessibleDescription = "Display name for the username handshake. This name is not authenticated."
    };
    private readonly TextBox _serverAddress = new()
    {
        Name = "ServerAddress",
        AccessibleName = "Server IPv4 address",
        Text = "127.0.0.1"
    };
    private readonly NumericUpDown _port = new()
    {
        Name = "Port",
        AccessibleName = "Server port",
        Minimum = 1,
        Maximum = 65535,
        Value = 5000
    };

    private readonly Button _connectButton = UiLayout.ActionButton("&Connect", "Connect");
    private readonly Button _disconnectButton = UiLayout.ActionButton("&Disconnect", "Disconnect");

    public ClientForm() : base("Client")
    {
        _client = new(OnConnectionAsync, onMessage: OnMessageAsync);
        BuildLayout();
        CompleteLayout();
        _connectButton.Click += ConnectClicked;
        _disconnectButton.Click += DisconnectClicked;
        _username.TextChanged += (_, _) => RenderConnection(_client.Snapshot);
        _serverAddress.TextChanged += (_, _) => RenderConnection(_client.Snapshot);
        _port.ValueChanged += (_, _) => RenderConnection(_client.Snapshot);
        RenderConnection(_client.Snapshot);
    }
}
