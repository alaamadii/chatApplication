using ChatApplication.Presentation.Layout;

namespace ChatApplication.Client.Forms;

internal sealed partial class ClientForm
{
    private void BuildLayout()
    {
        _username.Font = Fonts.Body;
        _serverAddress.Font = Fonts.Body;
        _port.Font = Fonts.Body;
        UiLayout.AddRow(Sidebar, UiLayout.Text("Your connection", Fonts.Section));
        UiLayout.AddRow(Sidebar, UiLayout.Text("Prepare a trusted local server endpoint.", Fonts.Metadata, secondary: true));
        UiLayout.AddRow(Sidebar, UiLayout.Field("&Username", _username));
        UiLayout.AddRow(Sidebar, UiLayout.Field("Server &IPv4 address", _serverAddress));
        UiLayout.AddRow(Sidebar, UiLayout.Field("&Port", _port));
        UiLayout.AddRow(Sidebar, UiLayout.Actions(_connectButton, _disconnectButton));
        UiLayout.AddRow(Sidebar, UiLayout.Text(
            "Connect completes after the server accepts your name. Messages are kept only in this window.",
            Fonts.Metadata, secondary: true));
    }
}
