using System.Globalization;
using System.Net;

namespace ChatApplication.Networking;

public static class EndpointInput
{
    public static bool TryCreate(string input, int port, out IPEndPoint? endpoint)
    {
        endpoint = null;
        var parts = input.Trim(' ').Split('.');
        if (port is < 1 or > 65535 || parts.Length != 4
            || parts.Any(part => part.Length == 0 || part.Any(character => character is < '0' or > '9')
                || !byte.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out _)))
        {
            return false;
        }
        endpoint = new(new IPAddress(parts.Select(part => byte.Parse(part, CultureInfo.InvariantCulture)).ToArray()), port);
        return true;
    }
}
