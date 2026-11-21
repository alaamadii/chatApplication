using ChatApplication.Presentation.Layout;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm
{
    private void BuildLayout()
    {
        _bindAddress.Font = Fonts.Body;
        _port.Font = Fonts.Body;
        UiLayout.AddRow(Sidebar, UiLayout.Text("Server console", Fonts.Section));
        UiLayout.AddRow(Sidebar, UiLayout.Text("Choose a local listener endpoint.", Fonts.Metadata, secondary: true));
        UiLayout.AddRow(Sidebar, UiLayout.Field("&Bind IPv4 address", _bindAddress));
        UiLayout.AddRow(Sidebar, UiLayout.Field("&Port", _port));
        UiLayout.AddRow(Sidebar, UiLayout.Actions(_startButton, _stopButton));
        UiLayout.AddRow(Sidebar, UiLayout.Text(
            "Start a local listener, then connect one client.",
            Fonts.Metadata, secondary: true));
        UiLayout.AddRow(Sidebar, UiLayout.Text("Client information (P04)", Fonts.Section));
        _peerSummary = UiLayout.Text("No accepted peer. Detailed client information arrives in P04.", Fonts.Metadata, secondary: true);
        _peerSummary.Name = "PeerSummary";
        UiLayout.AddRow(Sidebar, _peerSummary);

        _clients.Columns.Add("Username", 100);
        _clients.Columns.Add("Endpoint", 120);
        _clients.Columns.Add("Connected at", 130);
        _clients.Columns.Add("State", 90);
        UiLayout.AddRow(Sidebar, _clients);
    }
}
