using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging.Components;
using Flow.Automation.Services.Abstractions;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Listeners;

/// <summary>
/// Listeners for underlying messages are used to send messages to upper-level components.
/// Its function is familiar to <see cref="IObservable{T}"/> in the Rx framework.
/// </summary>
public abstract class Listener<TMessage> : IListener<TMessage> 
    where TMessage : notnull
{
    private readonly List<IObserver<TMessage>> _registeredObservers = [];
    
    /// <inheritdoc />
    public abstract string Identifier { get; set; }

    /// <inheritdoc />
    public abstract string Description { get; set; }

    /// <inheritdoc />
    public ProcessorStatus Status { get; } = ProcessorStatus.Ready;

    /// <inheritdoc />
    public abstract void Initialize();

    /// <inheritdoc />
    public abstract Task StartAsync();

    /// <inheritdoc />
    public abstract Task StopAsync();

    /// <summary>
    /// Evaluate whether this <see cref="obj">situation</see> meets the <see cref="condition">condition</see>.
    /// </summary>
    protected virtual bool EvaluateCondition<TObj>(ICondition condition, TObj? obj = default)
    {
        if (condition is IParamCondition<TObj> ps)
            return ps.Evaluate(obj);
        
        return condition.Result;
    }

    /// <summary>
    /// Notify all observers have been registered of a new message.
    /// </summary>
    protected virtual void BroadcastMessage(TMessage message)
    {
        foreach (var observer in _registeredObservers)
            observer.OnNext(message);
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<TMessage> observer)
    {
        var subscription = new DisposableSubscription(this, observer);
        _registeredObservers.Add(observer);
        return subscription;
    }

    /// <summary>
    /// Dispose a subscription between a listener and an observer.
    /// </summary>
    public void Unsubscribe(IDisposable subscription)
    {
        if (subscription is not DisposableSubscription s)
            return;

        if (!_registeredObservers.Contains(s.Observer))
            return;

        subscription.Dispose();
    }
}