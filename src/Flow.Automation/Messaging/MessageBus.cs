using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Flow.Automation.Messaging;

/// <summary>
/// Message bus implementation using <see cref="System.Reactive.Subjects.Subject"/> (Rx.NET).
/// </summary>
/// <typeparam name="T"></typeparam>
public class MessageBus<T> : IObserver<T>, IObservable<T>
{
    protected readonly Subject<T> Subject = new();

    public IObservable<T> Messages => Subject.AsObservable();

    /// <inheritdoc />
    public virtual void OnNext(T value)
        => Subject.OnNext(value);

    /// <remarks>
    /// Process the internal errors of message bus.
    /// Even if there are errors or completion messages from the IObservable,
    /// please prioritize using <see cref="OnCompleted"/>.
    /// </remarks>
    /// <inheritdoc />
    public virtual void OnError(Exception error)
    {
        Subject.OnError(error);
        OnCompleted();
    }

    /// <remarks>
    /// Terminate the message bus.
    /// Even if there are errors or completion messages from the IObservable,
    /// please prioritize using <see cref="OnCompleted"/>.
    /// </remarks>
    /// <inheritdoc />
    public virtual void OnCompleted()
    {
        Subject.OnCompleted();
    }

    /// <inheritdoc />
    public virtual IDisposable Subscribe(IObserver<T> observer)
        => Messages.Subscribe(observer);

    public virtual IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        => Messages.Subscribe(onNext, onError, onCompleted);

    /// <summary>
    /// Cancel the subscription.
    /// </summary>
    /// <param name="subscription">
    /// Returned value from <see cref="IObservable{T}.Subscribe">method Subscribe(...)</see>,
    /// which always used as a handle.
    /// </param>
    public virtual void Unsubscribe(IDisposable subscription)
        => subscription.Dispose();
}