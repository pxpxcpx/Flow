using Flow.Automation.Services.Listeners;

namespace Flow.Automation.Messaging;

// TODO: Better way to route the message and the identifier.

/// <summary>
/// Dispatch the message to handler.
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class MessageRouter<THandler>
    : IObservable<ListenerEventMessage>, IObserver<ListenerEventMessage>
    where THandler : IObserver<ListenerEventMessage>
{
    private bool _disposed;

    private MessageBus<ListenerEventMessage> _bus;

    public IObservable<ListenerEventMessage> Messages => _bus.Messages;

    public Exception? Exception { get; private set; }

    /// <summary>
    /// Event id and its handler.
    /// </summary>
    /// <remarks>The ID will be used as a shared key for the condition and its handler.</remarks>
    /// <seealso cref="IListener.Conditions"/>
    public Dictionary<Guid, THandler[]> Handlers { get; private set; }

    public MessageRouter(Dictionary<Guid, THandler[]> handlers)
    {
        _bus = new MessageBus<ListenerEventMessage>();
        _bus.Subscribe(this);

        Handlers = handlers;
    }

    /// <summary>
    /// Call all observers and observables that the router will be shut down.
    /// </summary>
    public void OnCompleted()
    {
        _bus.OnCompleted();
    }

    /// <summary>
    /// Call router to handle the error.
    /// This may shut down the router.
    /// </summary>
    /// <param name="error"></param>
    public void OnError(Exception error)
    {
        Exception = error;
        OnCompleted();
    }

    /// <summary>
    /// Response next message,
    /// call the observer corresponding to the event id to handle the message.
    /// </summary>
    /// <param name="value"></param>
    public void OnNext(ListenerEventMessage value)
    {
        Handlers.TryGetValue(value.EventArgs.EventId, out var handlers);

        if (handlers is not null or { Length: 0 })
            return;

        foreach (var handler in handlers!)
            handler.OnNext(value);
    }

    /// <summary>
    /// Ignore the event type and id, subscribe all events received.
    /// </summary>
    /// <remarks>
    /// To add specific event type and related handler,
    /// use <see cref="AddEventHandlerPair"/> or <see cref="AddEventHandlerPairs"/>.
    /// </remarks>
    /// <param name="observer"></param>
    /// <returns></returns>
    public IDisposable Subscribe(IObserver<ListenerEventMessage> observer)
        => _bus.Subscribe(observer);

    public IDisposable SubscribeTo(Guid eventId, IObserver<ListenerEventMessage> observer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add specific event id and related handlers to handlers list.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="handler"></param>
    public void AddEventHandlerPair(Guid eventId, THandler[] handler) 
        => Handlers.TryAdd(eventId, handler);

    /// <summary>
    /// Add multiple events and their related handlers to handlers list.
    /// </summary>
    /// <param name="pairs"></param>
    public void AddEventHandlerPairs(Dictionary<Guid, THandler[]> pairs)
    {
        foreach (var pair in pairs) 
            Handlers.TryAdd(pair.Key, pair.Value);
    }

    private sealed class MessageRouterDisposableHandle : IDisposable
    {
        public void Dispose()
        {
            // TODO 在此释放托管资源
        }
    }
}