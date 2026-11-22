namespace ChatApplication.Networking;

public enum ConnectionState
{
    Stopped, Starting, Listening, Stopping,
    Disconnected, Connecting, Handshaking, Connected, Disconnecting
}

public sealed record ConnectionSnapshot(
    ConnectionState State, string? UserName = null, Guid? SessionId = null, string? ErrorCode = null,
    ConnectedClientInfo? ClientInfo = null);
