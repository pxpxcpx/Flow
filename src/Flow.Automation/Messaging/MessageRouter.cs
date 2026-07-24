using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace Flow.Automation.Messaging;

/// <summary>
/// Dispatch the message to handler.
/// </summary>
public class MessageRouter : IObservable<object>, IObserver<object>, IDisposable
{
    private int _isDisposed;

    private Subject<object> _broadcastSubject;

    private ConcurrentDictionary<Type, ISubjectContainer> _subjects;

    public MessageRouter()
    {
        _isDisposed = 0;
        _broadcastSubject = new Subject<object>();
        _subjects = new ConcurrentDictionary<Type, ISubjectContainer>();
    }

    /// <summary>
    /// Register subject container to handle specific type messages.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="subjectContainer"></param>
    public void Register(Type messageType, ISubjectContainer subjectContainer)
        => _subjects.TryAdd(messageType, subjectContainer);

    /// <summary>
    /// Create a new subject and register it with specific type to handle messages.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public void Register<TMessage>()
    {
        var s = new Subject<TMessage>();
        var sc = new SubjectContainer<TMessage>(s);
        Register(typeof(TMessage), sc);
    }

    /// <summary>
    /// Register subject to handle specific type messages.
    /// </summary>
    /// <param name="subject"></param>
    /// <typeparam name="TMessage"></typeparam>
    public void Register<TMessage>(Subject<TMessage> subject)
    {
        var sc = new SubjectContainer<TMessage>(subject);
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
        if (!_subjects.TryGetValue(messageType, out var subjectContainer))
            return false;

        subjectContainer.OnCompleted();
        subjectContainer.Dispose();
        
        return _subjects.TryRemove(messageType, out _);
    }

    /// <summary>
    /// Get subject from the container by using the key.
    /// </summary>
    /// <typeparam name="TMessage">Type of Message.</typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Message type not registered.</exception>
    /// <exception cref="InvalidOperationException">Internal error, the type of the subject doesn't match.</exception>
    private Subject<TMessage> GetSubjectContainer<TMessage>()
    {
        var type = typeof(TMessage);

        if (!_subjects.TryGetValue(type, out var s))
            throw new KeyNotFoundException();

        if (s.Object is not Subject<TMessage> subject)
            throw new InvalidOperationException();

        return subject;
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
        GetSubjectContainer<TMessage>().OnNext(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    public void OnCompleted<TMessage>()
        where TMessage : notnull
        => GetSubjectContainer<TMessage>().OnCompleted();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    public void OnError<TMessage>(Exception message)
        where TMessage : notnull
        => GetSubjectContainer<TMessage>().OnError(message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="observer"></param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    /// <returns></returns>
    public IDisposable Subscribe<TMessage>(IObserver<TMessage> observer)
        where TMessage : notnull
        => GetSubjectContainer<TMessage>().Subscribe(observer);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="onNext">Action to handle the message.</param>
    /// <param name="onError">Action to handle the error.</param>
    /// <param name="onCompleted">Action to handle the completed signal.</param>
    /// <typeparam name="TMessage">The key to identify observers.</typeparam>
    /// <returns></returns>
    public IDisposable Subscribe<TMessage>(Action<TMessage> onNext, Action<Exception> onError, Action onCompleted)
        where TMessage : notnull
        => GetSubjectContainer<TMessage>().Subscribe(onNext, onError, onCompleted);

    /// <summary>
    /// Disconnect the subscription.
    /// </summary>
    /// <param name="subscription"></param>
    public void Unsubscribe(IDisposable subscription)
        => subscription.Dispose();
    
    /// <summary>
    /// Send a message to the router. The message will only be broadcasted.
    /// </summary>
    void IObserver<object>.OnNext(object value)
    {
        _broadcastSubject.OnNext(value);
        
        var type = value.GetType();
        if (!_subjects.TryGetValue(type, out var subjectContainer))
            return;
        
        subjectContainer.OnNext(value);
    }

    /// <summary>
    /// Terminate the router.
    /// This will stop all activities(OnNext, OnError...) of the router.
    /// </summary>
    void IObserver<object>.OnCompleted()
    {
        _broadcastSubject.OnCompleted();
        
        foreach (var subjectContainer in _subjects.Values)
            subjectContainer.OnCompleted();
    }

    /// <summary>
    /// Handle internal error, this may cause the termination of the router.
    /// </summary>
    /// <param name="error">Internal error.</param>
    void IObserver<object>.OnError(Exception error)
    {
        _broadcastSubject.OnError(error);
        
        foreach (var subjectContainer in _subjects.Values)
            subjectContainer.OnError(error);
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

        foreach (var subjectContainer in _subjects.Values)
        {
            try
            {
                subjectContainer.OnCompleted();
                subjectContainer.Dispose();
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