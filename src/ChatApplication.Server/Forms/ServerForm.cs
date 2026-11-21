using ChatApplication.Presentation;

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
        AccessibleDescription = "No clients. Client information is unavailable until connections are implemented.",
        View = View.Details,
        Height = 96,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 0, 0, 12),
        TabStop = false,
        FullRowSelect = true,
        HideSelection = false
    };

    public ServerForm() : base("Server")
    {
        BuildLayout();
        CompleteLayout();
    }
}
