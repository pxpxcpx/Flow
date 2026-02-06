namespace Flow.Automation.Messaging;

// TODO: Better way to route the message and the identifier.

/// <summary>
/// 
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class MessageRouter<THandler> : MessageBus<ListenerEventMessage>
    where THandler : IObserver<ListenerEventMessage>
{
    private bool _disposed;
    private readonly Dictionary<Guid, THandler[]> _handlers;

    public MessageRouter(Dictionary<Guid, THandler[]> handlers)
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

    public override void OnNext(ListenerEventMessage value)
    {
        _handlers.TryGetValue(value.EventArgs.ConditionId, out var handlers);

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
