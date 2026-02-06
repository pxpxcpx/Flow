using Flow.Automation.Services.Listeners;

namespace Flow.Automation.Messaging;

// TODO: Better way to route the message and the identifier.

/// <summary>
/// Dispatch the message to handler.
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class MessageRouter<THandler> : MessageBus<ListenerEventMessage>
    where THandler : IObserver<ListenerEventMessage>
{
    private bool _disposed;
    
    /// <summary>
    /// Condition id and its handler.
    /// </summary>
    /// <remarks>The ID will be used as a shared key for the condition and its handler.</remarks>
    /// <seealso cref="IListener.Conditions"/>
    public readonly Dictionary<Guid, THandler[]> Handlers;

    public MessageRouter(Dictionary<Guid, THandler[]> handlers)
    {
        Handlers = handlers;
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
        Handlers.TryGetValue(value.EventArgs.ConditionId, out var handlers);

        if (handlers is not null or { Length: 0 })
            return;

        foreach (var handler in handlers!)
            handler.OnNext(value);
    }
    
    public override void Dispose()
    {
        if (_disposed) return;
        
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
