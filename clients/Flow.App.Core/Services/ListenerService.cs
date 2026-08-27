using Flow.App.Shared.Abstractions;
using Flow.Automation.Messaging.Components;
using Microsoft.Extensions.Hosting;

namespace Flow.App.Core.Services;

/// <summary>
/// Service to run listeners background.
/// </summary>
public sealed class ListenerService : BackgroundService, IListenerService
{
    private readonly List<IListenerRouterAdapter> _listenerRouterAdapters;
    private readonly MessageRouter _router;
    private CancellationTokenSource _cts;

    public ListenerService(
        List<IListenerRouterAdapter> listenerRouterAdapters, MessageRouter router, CancellationTokenSource cts)
    {
        _listenerRouterAdapters = listenerRouterAdapters;
        _router = router;
        _cts = cts;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var lra in _listenerRouterAdapters)
            lra.Initialize();
        
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await base.StartAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var lra in _listenerRouterAdapters)
            await lra.StopAsync();
        
        await _cts.CancelAsync();
        await base.StopAsync(cancellationToken);
         
        ((IObserver<object>)_router).OnCompleted();
    }
    
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var lra in _listenerRouterAdapters)
            lra.Subscribe(_router);

        // Keep the service alive.
        var tcs = new TaskCompletionSource<bool>();
        stoppingToken.Register(() => tcs.TrySetResult(true));
        await tcs.Task;
    }
}
