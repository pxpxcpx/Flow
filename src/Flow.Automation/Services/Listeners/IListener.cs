using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Listeners;

public interface IListener
{
    string Name { get; }
    
    Dictionary<Guid, ICondition> Conditions { get; }
    
    IObservable<ListenerEventMessage> EventStream { get; }

    void Initialize();
    
    Task StartAsync();
    
    Task StopAsync();
    
    ProcessorStatus Status { get; }
}