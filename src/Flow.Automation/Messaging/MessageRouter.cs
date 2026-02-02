namespace Flow.Automation.Messaging;

public class MessageRouter<TKey, THandler> : MessageBus<TKey>
    where TKey : notnull
    where THandler : IObserver<TKey>
{
    private bool _disposed;
    private readonly Dictionary<TKey, THandler[]> _handlers;

    public MessageRouter(Dictionary<TKey, THandler[]> handlers)
    {
        _handlers = handlers;
    }

    public override void OnCompleted()
    {
        Dispose();
    }

    public override void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public override void OnNext(TKey value)
    {
        _handlers.TryGetValue(value, out var handlers);

        if (handlers is not null or { Length: 0 })
            return;

        foreach (var handler in handlers!)
            handler.OnNext(value);
    }
    
    public override void Dispose()
    {
        if (!_disposed)
            GC.SuppressFinalize(this);
    }
}
