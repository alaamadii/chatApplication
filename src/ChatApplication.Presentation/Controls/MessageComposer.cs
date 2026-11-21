namespace ChatApplication.Presentation.Controls;

public sealed class MessageComposer : TextBox
{
    private bool _composing;
    public event EventHandler? SendRequested;

    public MessageComposer()
    {
        Multiline = true;
        AcceptsReturn = true;
        AcceptsTab = false;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x010D) { _composing = true; } // WM_IME_STARTCOMPOSITION
        if (m.Msg == 0x010E) { _composing = false; } // WM_IME_ENDCOMPOSITION
        base.WndProc(ref m);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Enter && !_composing)
        {
            SendRequested?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
