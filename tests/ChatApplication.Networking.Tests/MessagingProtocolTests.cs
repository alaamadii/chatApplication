using System.Text;
using System.Text.Json.Nodes;
using ChatApplication.Core;
using ChatApplication.Networking.Protocol;
using Xunit;

namespace ChatApplication.Networking.Tests;

public sealed class MessagingProtocolTests
{
    [Fact]
    public async Task Chat_and_message_round_trip_through_coalesced_frames()
    {
        const string text = "  مرحبا 👋 & Türkçe\r\nsecond\tline  ";
        var message = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Client, "Alaa", text, DateTimeOffset.UnixEpoch);
        using var stream = new MemoryStream();
        using var channel = new FramedConnection(stream);
        await channel.WriteAsync(ConnectionPacket.Chat(text));
        await channel.WriteAsync(ConnectionPacket.AcceptedMessage(message));
        stream.Position = 0;
        Assert.Equal(text, (await channel.ReadAsync(PacketPhase.ServerAccepted))!.Text);
        Assert.Equal(message, (await channel.ReadAsync(PacketPhase.ClientAccepted))!.Message);
        Assert.Null(await channel.ReadAsync(PacketPhase.ClientAccepted));
    }

    [Theory]
    [InlineData("messageId", null)]
    [InlineData("messageId", "00000000-0000-0000-0000-000000000000")]
    [InlineData("authorRole", "admin")]
    [InlineData("authorRole", "Server")]
    [InlineData("authorName", " Other ")]
    [InlineData("authorName", "Server")]
    [InlineData("text", null)]
    [InlineData("text", "  ")]
    [InlineData("text", "a\0b")]
    [InlineData("sentAtUtc", "2026-11-21T12:00:00+03:00")]
    [InlineData("sentAtUtc", "yesterday")]
    [InlineData("sentAtUtc", "2026-11-21T12:00:00")]
    public void Rejects_missing_or_invalid_authoritative_fields(string field, string? value)
    {
        var message = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Client, "Alaa", "hello", DateTimeOffset.UnixEpoch);
        var node = JsonNode.Parse(MessageProtocol.Encode(ConnectionPacket.AcceptedMessage(message)))!.AsObject();
        if (value is null) { node.Remove(field); } else { node[field] = value; }
        Assert.Throws<ConnectionFailureException>(() =>
            MessageProtocol.Decode(Encoding.UTF8.GetBytes(node.ToJsonString()), PacketPhase.ClientAccepted));
    }

    [Fact]
    public void Chat_ignores_forged_identity_and_timestamp_but_rejects_wrong_direction()
    {
        var packet = MessageProtocol.Decode(Encoding.UTF8.GetBytes(
            """{"version":1,"type":"chat","text":"hello","authorName":"Victim","sentAtUtc":"forged","messageId":"forged"}"""),
            PacketPhase.ServerAccepted);
        Assert.Equal(ConnectionPacket.Chat("hello"), packet);
        var message = ConnectionPacket.AcceptedMessage(new(Guid.NewGuid(), MessageAuthorRole.Server,
            "Server", "hello", DateTimeOffset.UnixEpoch));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(MessageProtocol.Encode(message), PacketPhase.ServerAccepted));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(MessageProtocol.Encode(packet), PacketPhase.ClientAccepted));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(MessageProtocol.Encode(packet), PacketPhase.ServerHandshake));
    }

    [Fact]
    public void Encoded_maximum_unicode_text_fits_frame_and_invalid_surrogate_is_rejected()
    {
        var bytes = MessageProtocol.Encode(ConnectionPacket.AcceptedMessage(new(Guid.NewGuid(),
            MessageAuthorRole.Server, "Server", new string('ع', 2000), DateTimeOffset.UnixEpoch)));
        Assert.InRange(bytes.Length, 2000, MessageProtocol.MaximumPayloadBytes);
        Assert.Equal(2000, MessageProtocol.Decode(bytes, PacketPhase.ClientAccepted).Message!.Text.Length);
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(Encoding.UTF8.GetBytes(
            """{"version":1,"type":"chat","text":"\uD800"}"""), PacketPhase.ServerAccepted));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Encode(ConnectionPacket.Chat(new string('x', 2001))));
        Assert.Throws<ConnectionFailureException>(() => MessageProtocol.Decode(Encoding.UTF8.GetBytes(
            """{"version":1,"type":"chat","text":"x","text":"y"}"""), PacketPhase.ServerAccepted));
    }
}
