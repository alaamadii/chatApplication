using ChatApplication.Core;

namespace ChatApplication.Server.Forms;

internal sealed partial class ServerForm
{
    private ValueTask OnMessageAsync(ChatMessage message, CancellationToken token) =>
        DispatchUpdateAsync(() => AppendMessage(message), token);

    protected override Task<ChatMessage> SendMessageAsync(string text) => _server.SendAsync(text);
}
