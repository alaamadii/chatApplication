using ChatApplication.Core;
using Xunit;

namespace ChatApplication.Core.Tests;

public sealed class MessageTests
{
    [Theory]
    [InlineData("A")]
    [InlineData("  hello  ")]
    [InlineData("Türkçe: Çağrı")]
    [InlineData("مرحبا 👋\nSecond\tline\r\n& <script>")]
    public void Valid_text_is_preserved_verbatim(string text)
    {
        Assert.True(MessageRules.IsValid(text));
        var value = new ChatMessage(Guid.NewGuid(), MessageAuthorRole.Client, "Alaa", text, DateTimeOffset.UnixEpoch);
        Assert.Equal(text, value.Text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    [InlineData("a\0b")]
    [InlineData("a\u0001b")]
    [InlineData("a\u007fb")]
    public void Empty_and_control_text_is_rejected(string? text) => Assert.False(MessageRules.IsValid(text));

    [Fact]
    public void Length_counts_utf16_units_and_surrogates_must_pair()
    {
        Assert.True(MessageRules.IsValid(new string('x', 2000)));
        Assert.False(MessageRules.IsValid(new string('x', 2001)));
        Assert.True(MessageRules.IsValid(string.Concat(Enumerable.Repeat("😀", 1000))));
        Assert.False(MessageRules.IsValid(string.Concat(Enumerable.Repeat("😀", 1001))));
        Assert.False(MessageRules.IsValid(new string(['x', '\ud800'])));
        Assert.False(MessageRules.IsValid(new string(['\udc00', 'x'])));
        Assert.False(MessageRules.IsValid(new string(['\ud800', 'x'])));
    }

    [Fact]
    public void Model_requires_authoritative_id_role_canonical_name_text_and_utc()
    {
        var id = Guid.NewGuid();
        var utc = DateTimeOffset.UnixEpoch;
        Assert.Throws<ArgumentException>(() => new ChatMessage(Guid.Empty, MessageAuthorRole.Client, "Alaa", "x", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, (MessageAuthorRole)9, "Alaa", "x", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, MessageAuthorRole.Client, " Alaa ", "x", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, MessageAuthorRole.Client, "Server", "x", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, MessageAuthorRole.Server, "Alaa", "x", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, MessageAuthorRole.Server, "Server", "", utc));
        Assert.Throws<ArgumentException>(() => new ChatMessage(id, MessageAuthorRole.Server, "Server", "x", utc.ToOffset(TimeSpan.FromHours(3))));
        var message = new ChatMessage(id, MessageAuthorRole.Server, "Server", "x", utc);
        Assert.Equal(id, message.MessageId);
        Assert.Equal(utc, message.SentAtUtc);
    }
}
