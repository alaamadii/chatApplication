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
        UiLayout.AddRow(Sidebar, UiLayout.Actions(
            UiLayout.UnavailableButton("S&tart server", "StartServer"),
            UiLayout.UnavailableButton("St&op server", "StopServer")));
        UiLayout.AddRow(Sidebar, UiLayout.Text(
            "Listener controls are not implemented. Available in P02.",
            Fonts.Metadata, secondary: true));
        UiLayout.AddRow(Sidebar, UiLayout.Text("Connected client", Fonts.Section));
        UiLayout.AddRow(Sidebar, UiLayout.Text(
            "No clients connected. No client information is available.",
            Fonts.Metadata, secondary: true));

        _clients.Columns.Add("Username", 100);
        _clients.Columns.Add("Endpoint", 120);
        _clients.Columns.Add("Connected at", 130);
        _clients.Columns.Add("State", 90);
        UiLayout.AddRow(Sidebar, _clients);
    }
}
