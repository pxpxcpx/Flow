using System.Reactive.Subjects;
using Flow.Automation.Messaging.Abstractions;

namespace Flow.Automation.Messaging.Components;

/// <summary>
/// Represents a disposable connected subscription between observer and observable object.
/// </summary>
public sealed class DisposableSubscription : IDisposable
{
    private volatile int _disposed;

    /// <summary>
    /// Use this when the observable object has its internal logic to handle the subscription.
    /// <example>
    /// A message router using <see cref="Subject{T}"/> to handle subscription inside.
    /// </example>
    /// </summary>
    public readonly IDisposable? InternalSubscription;

    /// <summary>
    /// Observable object, must implement <see cref="ISubscriptionManaged.Unsubscribe"/> method.
    /// </summary>
    public readonly ISubscriptionManaged Observable;

    /// <summary>
    /// To avoid the generic type, use object to storage the observer.
    /// </summary>
    public readonly object Observer;

    private readonly CancellationTokenSource? _cancellationTokenSource;

    public DisposableSubscription(ISubscriptionManaged observable, object observer,
        IDisposable? internalSubscription = null, CancellationTokenSource? cts = null)
    {
        _disposed = 0;
        _cancellationTokenSource = cts;
        Observable = observable;
        Observer = observer;
        InternalSubscription = internalSubscription;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 1)
            return;

        Observable.Unsubscribe(this);
    }
}