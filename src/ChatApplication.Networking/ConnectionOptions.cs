namespace ChatApplication.Networking;

public sealed record ConnectionOptions
{
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan HandshakeTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan FrameTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan SendTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan ShutdownTimeout { get; init; } = TimeSpan.FromSeconds(5);

    internal void Validate()
    {
        foreach (var duration in new[] { ConnectTimeout, HandshakeTimeout, FrameTimeout, SendTimeout, ShutdownTimeout })
        {
            if (duration <= TimeSpan.Zero || duration > TimeSpan.FromMinutes(1))
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Deadlines must be positive and at most one minute.");
            }
        }
    }
}
