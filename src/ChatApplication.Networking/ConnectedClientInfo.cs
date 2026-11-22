namespace ChatApplication.Networking;

// Server-local accepted-session data; never a client-supplied wire claim.
public sealed record ConnectedClientInfo(
    Guid SessionId, string UserName, string RemoteAddress, int RemotePort, DateTimeOffset ConnectedAtUtc)
{
    public string RemoteEndpoint => $"{RemoteAddress}:{RemotePort}";
    public ConnectionState State => ConnectionState.Connected;
}
