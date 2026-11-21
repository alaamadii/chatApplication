using ChatApplication.Core;

namespace ChatApplication.Networking;

internal sealed class MessageUpdates(
    Func<ChatMessage, CancellationToken, ValueTask>? observer, ConnectionOptions options)
{
    public async ValueTask PublishAsync(ChatMessage message, CancellationToken cancellationToken)
    {
        if (observer is null) { return; }
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(options.SendTimeout);
        try { await observer(message, deadline.Token).ConfigureAwait(false); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception) { throw new ConnectionFailureException("notification_failed", exception); }
    }
}
