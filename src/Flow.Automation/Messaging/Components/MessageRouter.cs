using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Flow.Automation.Messaging.Abstractions;

namespace Flow.Automation.Messaging.Components;

/// <summary>
/// Dispatches messages to handler by the message type.
/// </summary>
public class MessageRouter : IObservable<object>, IObserver<object>, ISubscriptionManaged, IDisposable
{
    private int _isDisposed;

    private readonly Subject<object> _broadcastSubject;

    private readonly ConcurrentDictionary<Type, ISubjectWrapper> _subjects;

    private readonly ConcurrentDictionary<Type, (object, object)> _typedRoutersCache;

    public MessageRouter()
    {
        _typedRoutersCache = new();
        _isDisposed = 0;
        _broadcastSubject = new Subject<object>();
        _subjects = new ConcurrentDictionary<Type, ISubjectWrapper>();
    }

    /// <summary>
    /// Register subject container to handle specific type messages.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="subjectWrapper"></param>
    public void Register(Type messageType, ISubjectWrapper subjectWrapper)
        => _subjects.TryAdd(messageType, subjectWrapper);

    /// <summary>
    /// Create a new subject and register it with specific type to handle messages.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public void Register<TMessage>()
    {
        var s = new Subject<TMessage>();
        var sc = new SubjectWrapper<TMessage>(s);
        Register(typeof(TMessage), sc);
    }

    /// <summary>
    /// Register subject to handle specific type messages.
    /// </summary>
    /// <param name="subject"></param>
    /// <typeparam name="TMessage"></typeparam>
    public void Register<TMessage>(Subject<TMessage> subject)
    {
        var sc = new SubjectWrapper<TMessage>(subject);
        Register(typeof(TMessage), sc);
    }

    /// <summary>
    /// Terminate the corresponding subject and remove it from the router.
    /// The routing of specific messages will be stopped.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public bool RemoveRegistration<TMessage>()
        => RemoveRegistration(typeof(TMessage));

    /// <summary>
    /// Terminate the corresponding subject and remove it from the router.
    /// The routing of specific messages will be stopped.
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    public bool RemoveRegistration(Type messageType)
    {
        if (!_subjects.TryGetValue(messageType, out var subjectWrapper))
            return false;

        subjectWrapper.OnCompleted();
        subjectWrapper.Dispose();

        return _subjects.TryRemove(messageType, out _);
    }

    private ISubjectWrapper GetSubjectWrapper(Type messageType)
    {
        if (!_subjects.TryGetValue(messageType, out var s))
            throw new KeyNotFoundException();

        return s;
    }

    /// <summary>
    /// Get subject from the container by using the key.
    /// </summary>
    /// <typeparam name="TMessage">Type of Message.</typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Message type not registered.</exception>
    /// <exception cref="InvalidOperationException">Internal error, the type of the subject doesn't match.</exception>
    private Subject<TMessage> GetSubject<TMessage>()
    {
        if (GetSubjectWrapper(typeof(TMessage)).Object is not Subject<TMessage> subject)
            throw new InvalidOperationException();

        return subject;
    }

    private void AddRouterHandles<T>() 
        where T : notnull
    {
        var observer = new ObserverRouter<T>(this);
        var observable = new ObservableRouter<T>(this);
        _typedRoutersCache.TryAdd(typeof(T), (observable, observer));
    }

    /// <summary>
    /// Gets a handle implements <see cref="IObservable{T}"/> of a message router.
    /// </summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    private sealed class ObservableRouter<T> : IObservable<T> where T : notnull
    {
        private readonly MessageRouter _router;

        internal ObservableRouter(MessageRouter router)
        {
            _router = router;
        }

        public IDisposable Subscribe(IObserver<T> observer)
            => _router.Subscribe(observer);
    }

    /// <summary>
    /// Gets a handle implements <see cref="IObservable{T}"/> of a message router.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IObservable<T> AsObservable<T>() 
        where T : notnull
    {
        var t = typeof(T);
        
        if (!_typedRoutersCache.TryGetValue(t, out var pairs))
            AddRouterHandles<T>();
        
        return (ObservableRouter<T>)pairs.Item1;
    }

    /// <summary>
    /// Handle implements <see cref="IObserver{T}"/> of a message router.
    /// </summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    private sealed class ObserverRouter<T> : IObserver<T> where T : notnull
    {
        private readonly MessageRouter _router;

        internal ObserverRouter(MessageRouter router)
        {
            _router = router;
        }

        public void OnCompleted()
            => _router.OnCompleted<T>();

        public void OnError(Exception error)
            => _router.OnError<T>(error);

        public void OnNext(T value)
            => _router.OnNext(value);
    }

    /// <summary>
    /// Gets a handle implements <see cref="IObserver{T}"/> of a message router.
    /// </summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    /// <returns>Converting result.</returns>
    public IObserver<T> AsObserver<T>() 
        where T : notnull
    {
        var t = typeof(T);
        
        if (!_typedRoutersCache.TryGetValue(t, out var pairs))
            AddRouterHandles<T>();
        
        return (ObserverRouter<T>)pairs.Item2;
    }

    /// <summary>
    /// Notifies corresponding subscribed observers about the arrival of the specified element in the sequence.
    /// </summary>
    /// <param name="message">The value to send to specific subscribed observers.</param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    public void OnNext<TMessage>(TMessage message)
        where TMessage : notnull
    {
        _broadcastSubject.OnNext(message);
        GetSubject<TMessage>().OnNext(message);
    }

    /// <summary>
    /// Use the specified type of message to notify the corresponding subject to stop.
    /// </summary>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    public void OnCompleted<TMessage>()
        where TMessage : notnull
        => GetSubject<TMessage>().OnCompleted();

    /// <summary>
    /// Use the specified type of message to notify the corresponding subject to handle the error.
    /// </summary>
    /// <param name="err">Error message.</param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    public void OnError<TMessage>(Exception err)
        where TMessage : notnull
        => GetSubject<TMessage>().OnError(err);

    /// <summary>
    /// Subscribe to messages of a specific type.
    /// </summary>
    /// <param name="observer">Observer.</param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    /// <returns></returns>
    public IDisposable Subscribe<TMessage>(IObserver<TMessage> observer)
        where TMessage : notnull
        => new DisposableSubscription(this, observer, GetSubject<TMessage>().Subscribe(observer));

    /// <summary>
    /// Subscribe to messages of a specific type with action group.
    /// </summary>
    /// <param name="onNext">Action to handle the message.</param>
    /// <param name="onError">Action to handle the error.</param>
    /// <param name="onCompleted">Action to handle the completed signal.</param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    /// <returns></returns>
    public IDisposable Subscribe<TMessage>(Action<TMessage> onNext, Action<Exception> onError, Action onCompleted)
        where TMessage : notnull
        => GetSubject<TMessage>().Subscribe(onNext, onError, onCompleted);

    /// <summary>
    /// Disconnect the subscription.
    /// </summary>
    /// <param name="subscription"></param>
    public void Unsubscribe(IDisposable subscription)
    {
        if (subscription is not DisposableSubscription ds)
            return;

        if (ds.Observable != this)
            return;

        ds.InternalSubscription?.Dispose();
    }

    /// <summary>
    /// Send a message to the router. The message will only be broadcasted.
    /// </summary>
    void IObserver<object>.OnNext(object value)
    {
        _broadcastSubject.OnNext(value);

        var type = value.GetType();
        if (!_subjects.TryGetValue(type, out var subjectWrapper))
            return;

        subjectWrapper.OnNext(value);
    }

    /// <summary>
    /// Terminate the router.
    /// This will stop all activities(OnNext, OnError...) of the router.
    /// </summary>
    void IObserver<object>.OnCompleted()
    {
        _broadcastSubject.OnCompleted();

        foreach (var subjectWrapper in _subjects.Values)
            subjectWrapper.OnCompleted();
    }

    /// <summary>
    /// Handle internal error, this may cause the termination of the router.
    /// </summary>
    /// <param name="error">Internal error.</param>
    void IObserver<object>.OnError(Exception error)
    {
        _broadcastSubject.OnError(error);

        foreach (var subjectWrapper in _subjects.Values)
            subjectWrapper.OnError(error);
    }

    /// <summary>
    /// Ignore the message type, listen to all kinds of messages.
    /// </summary>
    /// <remarks><see cref="MessageBus{T}"/> has the same effect.</remarks>
    /// <param name="observer">Observer listen to all messages.</param>
    /// <returns></returns>
    IDisposable IObservable<object>.Subscribe(IObserver<object> observer)
        => _broadcastSubject.Subscribe(observer);

    /// <summary>
    /// Disconnect all subscriptions.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
            return;

        foreach (var subjectWrapper in _subjects.Values)
        {
            try
            {
                subjectWrapper.OnCompleted();
                subjectWrapper.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        _subjects.Clear();

        _broadcastSubject.Dispose();
    }
}