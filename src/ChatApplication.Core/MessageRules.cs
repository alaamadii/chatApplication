namespace ChatApplication.Core;

public static class MessageRules
{
    public const int MaximumLength = 2000;

    public static bool IsValid(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > MaximumLength) { return false; }
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (char.IsHighSurrogate(character))
            {
                if (++index >= text.Length || !char.IsLowSurrogate(text[index])) { return false; }
            }
            else if (char.IsLowSurrogate(character)
                || char.IsControl(character) && character is not ('\t' or '\r' or '\n'))
            {
                return false;
            }
        }
        return true;
    }
}
