using System.Globalization;
using ChatApplication.Networking;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm
{
    private void RenderClientInfo(ConnectedClientInfo? info)
    {
        _peerSummary.Text = info is null ? "No accepted peer." : $"Accepted peer: {info.UserName}.";
        _clients.TabStop = info is not null;
        if (info is null)
        {
            _clients.Items.Clear();
            _clients.AccessibleDescription = "No accepted client. Pending and rejected connections are not listed.";
            return;
        }
        var local = info.ConnectedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        var description = $"{info.UserName}; endpoint {info.RemoteEndpoint}; connected at {local}; {info.State}.";
        _clients.AccessibleDescription = description;
        if (_clients.Items.Count == 1 && Equals(_clients.Items[0].Tag, info)) { return; }
        _clients.BeginUpdate();
        try
        {
            _clients.Items.Clear();
            var row = new ListViewItem([info.UserName, info.RemoteEndpoint, local, info.State.ToString()])
            {
                Tag = info,
                ToolTipText = description
            };
            _clients.Items.Add(row);
            FitClientColumns();
        }
        finally { _clients.EndUpdate(); }
    }

    private void FitClientColumns()
    {
        if (!_clients.IsHandleCreated) { return; }
        using var graphics = _clients.CreateGraphics();
        var padding = (int)Math.Round(24 * _clients.DeviceDpi / 96F);
        for (var index = 0; index < _clients.Columns.Count; index++)
        {
            var width = TextRenderer.MeasureText(graphics, _clients.Columns[index].Text, _clients.Font).Width;
            foreach (ListViewItem row in _clients.Items)
            {
                width = Math.Max(width, TextRenderer.MeasureText(graphics, row.SubItems[index].Text, _clients.Font).Width);
            }
            _clients.Columns[index].Width = width + padding;
        }
    }
}
