namespace ChatApplication.Core;

public enum MessageAuthorRole { Client, Server }

public sealed record ChatMessage
{
    public Guid MessageId { get; }
    public MessageAuthorRole AuthorRole { get; }
    public string AuthorName { get; }
    public string Text { get; }
    public DateTimeOffset SentAtUtc { get; }

    public ChatMessage(Guid messageId, MessageAuthorRole authorRole, string authorName,
        string text, DateTimeOffset sentAtUtc)
    {
        var validAuthor = authorRole switch
        {
            MessageAuthorRole.Server => authorName == "Server",
            MessageAuthorRole.Client => UsernameRules.TryNormalize(authorName, out var normalized) && normalized == authorName,
            _ => false
        };
        if (messageId == Guid.Empty || !validAuthor || !MessageRules.IsValid(text)
            || sentAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Invalid authoritative message.");
        }
        MessageId = messageId;
        AuthorRole = authorRole;
        AuthorName = authorName;
        Text = text;
        SentAtUtc = sentAtUtc;
    }
}
