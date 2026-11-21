namespace ChatApplication.Presentation.Theme;

public sealed class OwnedFonts : IDisposable
{
    public Font Body { get; } = new("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    public Font Title { get; } = new("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
    public Font Section { get; } = new("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
    public Font Metadata { get; } = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Body.Dispose();
        Title.Dispose();
        Section.Dispose();
        Metadata.Dispose();
        _disposed = true;
    }
}
