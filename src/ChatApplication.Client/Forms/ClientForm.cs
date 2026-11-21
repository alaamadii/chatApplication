using ChatApplication.Presentation;

namespace ChatApplication.Client.Forms;

internal sealed partial class ClientForm : ChatWindow
{
    private readonly TextBox _username = new()
    {
        Name = "Username",
        AccessibleName = "Username",
        AccessibleDescription = "Editable display-name input. No username is accepted in this interface-only phase."
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

    public ClientForm() : base("Client")
    {
        BuildLayout();
        CompleteLayout();
    }
}
