namespace ChatApplication.Networking.Protocol;

public sealed record ConnectionPacket(string Type, string? UserName = null, Guid? SessionId = null, string? Code = null)
{
    public static ConnectionPacket Hello(string name) => new("hello", UserName: name);
    public static ConnectionPacket Welcome(string name, Guid session) => new("welcome", name, session);
    public static ConnectionPacket Error(string code) => new("error", Code: code);
    public static ConnectionPacket Disconnect() => new("disconnect");
}

public enum PacketPhase
{
    ServerHandshake,
    ClientHandshake,
    ServerAccepted,
    ClientAccepted
}
