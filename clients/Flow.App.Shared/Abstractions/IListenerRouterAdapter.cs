using Flow.Automation.Messaging.Components;
using Flow.Shared.Abstractions;

namespace Flow.App.Shared.Abstractions;

public interface IListenerRouterAdapter : IRecognizable, IWrapper
{
    void Initialize();
    
    Task StartAsync();
    
    Task StopAsync();

    IDisposable Subscribe(MessageRouter router);
}