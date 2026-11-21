namespace ChatApplication.Presentation;

public static class ConnectionFeedback
{
    public static string For(string? code) => code switch
    {
        null => "Connect to exchange messages. Enter sends; Shift+Enter adds a new line.",
        "invalid_name" => "Use 2–24 supported letters, digits, spaces, underscores or hyphens; Server/System are reserved.",
        "server_busy" => "The server already has a pending or accepted client. Disconnect it before trying again.",
        "invalid_packet" => "The peer sent an invalid or unsupported connection packet. Check that both apps use the same version.",
        "unsupported_version" => "The peer uses an unsupported protocol version. Use matching application versions.",
        "address_in_use" => "That address and port are already in use. Choose another port or stop your own listener.",
        "connection_refused" => "The server refused the connection. Check its address, port and listening status.",
        "operation_timeout" => "The connection operation timed out. Check the endpoint and try again explicitly.",
        "connection_lost" => "The connection ended unexpectedly. Check the server and reconnect explicitly.",
        _ => "The connection could not complete safely. Stop/disconnect and try again."
    };
}
