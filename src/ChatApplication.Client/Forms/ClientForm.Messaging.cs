using ChatApplication.Core;

namespace ChatApplication.Client.Forms;

internal sealed partial class ClientForm
{
    private ValueTask OnMessageAsync(ChatMessage message, CancellationToken token) =>
        DispatchUpdateAsync(() => AppendMessage(message), token);

    protected override void OnMessageSendFinished() => RenderConnection(_client.Snapshot);

    protected override Task<ChatMessage> SendMessageAsync(string text) => _client.SendAsync(text);
}
