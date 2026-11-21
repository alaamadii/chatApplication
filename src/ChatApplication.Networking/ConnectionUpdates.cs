namespace ChatApplication.Networking;

internal sealed class ConnectionUpdates(
    ConnectionState initial, ConnectionOptions options,
    Func<ConnectionSnapshot, CancellationToken, ValueTask>? observer) : IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private ConnectionSnapshot _current = new(initial);
    private bool _observerFailed;

    public ConnectionSnapshot Current => Volatile.Read(ref _current);

    public async Task PublishAsync(ConnectionSnapshot value, CancellationToken cancellationToken = default)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(options.ShutdownTimeout);
        await _gate.WaitAsync(deadline.Token).ConfigureAwait(false);
        try
        {
            deadline.Token.ThrowIfCancellationRequested();
            Volatile.Write(ref _current, value);
            if (observer is not null && !_observerFailed)
            {
                try { await observer(value, deadline.Token).ConfigureAwait(false); }
                catch (OperationCanceledException) when (deadline.IsCancellationRequested) { throw; }
                catch (Exception exception)
                {
                    _observerFailed = true;
                    Volatile.Write(ref _current, value with { ErrorCode = "notification_failed" });
                    throw new ConnectionFailureException("notification_failed", exception);
                }
            }
        }
        finally { _gate.Release(); }
    }

    public void Dispose() => _gate.Dispose();
}
