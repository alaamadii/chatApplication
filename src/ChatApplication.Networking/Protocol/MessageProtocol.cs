using System.Text;
using System.Text.Json;
using ChatApplication.Core;

namespace ChatApplication.Networking.Protocol;

public static class MessageProtocol
{
    public const int MaximumPayloadBytes = 16_384;
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    private static readonly HashSet<string> ErrorCodes =
        ["invalid_name", "server_busy", "invalid_packet", "unsupported_version"];

    public static ConnectionPacket Decode(ReadOnlySpan<byte> payload, PacketPhase phase)
    {
        try
        {
            if (payload.Length is < 1 or > MaximumPayloadBytes)
            {
                throw new ConnectionFailureException("invalid_packet");
            }

            // Decode strictly before parsing; JSON does not silently replace invalid UTF-8.
            var json = StrictUtf8.GetString(payload);
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 8 });
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new ConnectionFailureException("invalid_packet");
            }

            RejectDuplicates(root);
            if (!root.TryGetProperty("version", out var version) || !version.TryGetInt32(out var number))
            {
                throw new ConnectionFailureException("invalid_packet");
            }
            if (number != 1)
            {
                throw new ConnectionFailureException("unsupported_version");
            }

            var type = StringField(root, "type");
            var allowed = phase switch
            {
                PacketPhase.ServerHandshake => type == "hello",
                PacketPhase.ClientHandshake => type is "welcome" or "error",
                PacketPhase.ServerAccepted => type == "disconnect",
                PacketPhase.ClientAccepted => type is "disconnect" or "error",
                _ => false
            };
            if (!allowed)
            {
                throw new ConnectionFailureException("invalid_packet");
            }

            return type switch
            {
                "hello" => ConnectionPacket.Hello(StringField(root, "userName")),
                "welcome" => ReadWelcome(root),
                "error" => ReadError(root),
                "disconnect" => ConnectionPacket.Disconnect(),
                _ => throw new ConnectionFailureException("invalid_packet")
            };
        }
        catch (Exception exception) when (exception is JsonException or DecoderFallbackException
            or InvalidOperationException or FormatException)
        {
            throw new ConnectionFailureException("invalid_packet", exception);
        }
    }

    public static byte[] Encode(ConnectionPacket packet)
    {
        object value = packet.Type switch
        {
            "hello" when packet.UserName is not null => new { version = 1, type = "hello", userName = packet.UserName },
            "welcome" when packet.UserName is not null && packet.SessionId is { } id && id != Guid.Empty =>
                new { version = 1, type = "welcome", userName = packet.UserName, sessionId = id },
            "error" when packet.Code is not null && ErrorCodes.Contains(packet.Code) =>
                new { version = 1, type = "error", code = packet.Code },
            "disconnect" => new { version = 1, type = "disconnect" },
            _ => throw new ConnectionFailureException("invalid_packet")
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        if (bytes.Length > MaximumPayloadBytes)
        {
            throw new ConnectionFailureException("invalid_packet");
        }
        return bytes;
    }

    private static string StringField(JsonElement root, string field)
    {
        if (!root.TryGetProperty(field, out var value) || value.ValueKind != JsonValueKind.String)
        {
            throw new ConnectionFailureException("invalid_packet");
        }
        var text = value.GetString()!;
        // JsonDocument accepts escaped unpaired surrogates; the wire contract does not.
        try { StrictUtf8.GetByteCount(text); }
        catch (EncoderFallbackException exception) { throw new ConnectionFailureException("invalid_packet", exception); }
        return text;
    }

    private static ConnectionPacket ReadWelcome(JsonElement root)
    {
        var name = StringField(root, "userName");
        if (!UsernameRules.TryNormalize(name, out var normalized) || name != normalized
            || !Guid.TryParse(StringField(root, "sessionId"), out var session) || session == Guid.Empty)
        {
            throw new ConnectionFailureException("invalid_packet");
        }
        return ConnectionPacket.Welcome(name, session);
    }

    private static ConnectionPacket ReadError(JsonElement root)
    {
        var code = StringField(root, "code");
        if (!ErrorCodes.Contains(code))
        {
            throw new ConnectionFailureException("invalid_packet");
        }
        return ConnectionPacket.Error(code);
    }

    private static void RejectDuplicates(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name))
                {
                    throw new ConnectionFailureException("invalid_packet");
                }
                RejectDuplicates(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in element.EnumerateArray())
            {
                RejectDuplicates(child);
            }
        }
    }
}
