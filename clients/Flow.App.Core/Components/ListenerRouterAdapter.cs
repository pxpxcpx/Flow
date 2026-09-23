using Flow.App.Shared.Abstractions;
using Flow.Automation.Messaging.Components;
using Flow.Automation.Services.Abstractions;
using Flow.Automation.Services.Listeners;
using Flow.Shared.Infrastructures;

namespace Flow.App.Core.Components;

internal sealed class ListenerRouterAdapter<TMessage> : Wrapper, IListenerRouterAdapter, IObservable<TMessage>
    where TMessage : notnull
{
    private readonly Listener<TMessage> _listener;

    /// <inheritdoc />
    public override object Object
    {
        get => _listener;
        set => SetObject(value);
    }

    /// <inheritdoc />
    public string Identifier { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    public ListenerRouterAdapter(object listener) : base(listener)
    {
        if (listener is not IListener<TMessage>)
            throw new ArgumentException($"{nameof(listener)} must be of type {nameof(IListener<TMessage>)}");
        
        _listener = (Listener<TMessage>)listener;
        Identifier = _listener.Identifier;
        Description = _listener.Description;
    }

    public ListenerRouterAdapter(Type type, object listener) : base(type, listener)
    {
        if (listener is not IListener<TMessage>)
            throw new ArgumentException($"{nameof(listener)} must be of type {nameof(IListener<TMessage>)}");
        
        _listener = (Listener<TMessage>)listener;
        Identifier = _listener.Identifier;
        Description = _listener.Description;
    }

    /// <inheritdoc />
    public void Initialize()
        => _listener.Initialize();

    /// <inheritdoc />
    public Task StartAsync()
        => _listener.StartAsync();

    /// <inheritdoc />
    public Task StopAsync()
        => _listener.StopAsync();

    public IDisposable Subscribe(MessageRouter router)
        => _listener.Subscribe(router.AsObserver<TMessage>());

    public IDisposable Subscribe(IObserver<TMessage> listener)
        => _listener.Subscribe(listener);
}