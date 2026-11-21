using System.Globalization;

namespace ChatApplication.Core;

public static class UsernameRules
{
    public static bool TryNormalize(string? input, out string name)
    {
        name = "";
        if (input is null || input.Any(character =>
            char.GetUnicodeCategory(character) is UnicodeCategory.Control or UnicodeCategory.Format))
        {
            return false;
        }

        var trimmed = input.Trim(' ');
        if (trimmed.Length is < 2 or > 24
            || trimmed.Equals("Server", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("System", StringComparison.OrdinalIgnoreCase)
            || trimmed.Any(character => !char.IsLetterOrDigit(character) && character is not (' ' or '_' or '-')))
        {
            return false;
        }

        name = trimmed;
        return true;
    }
}
