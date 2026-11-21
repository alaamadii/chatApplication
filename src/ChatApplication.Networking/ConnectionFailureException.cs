using System.Net.Sockets;

namespace ChatApplication.Networking;

public sealed class ConnectionFailureException(string code, Exception? innerException = null)
    : IOException(code, innerException)
{
    public string Code { get; } = code;

    internal static string CodeFor(Exception exception) => exception switch
    {
        ConnectionFailureException failure => failure.Code,
        TimeoutException or OperationCanceledException => "operation_timeout",
        SocketException { SocketErrorCode: SocketError.AddressAlreadyInUse } => "address_in_use",
        SocketException { SocketErrorCode: SocketError.ConnectionRefused } => "connection_refused",
        SocketException or IOException or ObjectDisposedException => "connection_lost",
        _ => "unexpected_error"
    };
}
