using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Flow.Automation.Messaging;

/// <summary>
/// Message bus implementation using <see cref="Subject"/> (Rx.NET).
/// </summary>
/// <typeparam name="T"></typeparam>
public class MessageBus<T> : IObserver<T>, IObservable<T>, IDisposable
{
    protected readonly Subject<T> _subject = new();

    public IObservable<T> Messages => _subject.AsObservable();

    /// <inheritdoc />
    public virtual void OnNext(T value)
        => _subject.OnNext(value);

    /// <inheritdoc />
    public virtual void OnError(Exception error)
        => _subject.OnError(error);

    /// <inheritdoc />
    public virtual void OnCompleted()
        => _subject.OnCompleted();

    /// <inheritdoc />
    public virtual IDisposable Subscribe(IObserver<T> observer)
        => Messages.Subscribe(observer);

    public virtual IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        => Messages.Subscribe(onNext, onError, onCompleted);

    public virtual void Dispose()
    {
        _subject.Dispose();
        GC.SuppressFinalize(this);
    }
}