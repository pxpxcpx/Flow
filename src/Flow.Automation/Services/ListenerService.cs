using Flow.Automation.Messaging;
using Flow.Automation.Services.Listeners;
using Flow.Shared.Results;
using Microsoft.Extensions.Hosting;

namespace Flow.Automation.Services;

/// <summary>
/// Service to run listeners background.
/// </summary>
public class ListenerService : BackgroundService, IListenerService
{
    private readonly List<IListener> _listeners;
    private readonly MessageRouter<EventMessage, IObserver<EventMessage>> _router;
    private CancellationTokenSource _cts;

    public ListenerService(
        List<IListener> listeners, MessageRouter<EventMessage, IObserver<EventMessage>> router, CancellationTokenSource cts)
    {
        _listeners = listeners;
        _router = router;
        _cts = cts;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var listener in _listeners)
        {
            listener.Initialize();
        }
        
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await base.StartAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var listener in _listeners)
        {
            await listener.StopAsync();
        }
        
        await _cts.CancelAsync();
        await base.StopAsync(cancellationToken);
        
        _router.OnCompleted();
    }
    
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var listener in _listeners)
        {
            listener.EventStream.Subscribe(
                _router.OnNext,
                _router.OnError,
                _router.OnCompleted, 
                stoppingToken);
            listener.StartAsync().Await();
        }

        // Keep the service alive.
        var tcs = new TaskCompletionSource<bool>();
        stoppingToken.Register(() => tcs.TrySetResult(true));
        await tcs.Task;
    }

    public void RegisterListener(IListener listener)
    {
        _listeners.Add(listener);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _router.Dispose();
        _router.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
