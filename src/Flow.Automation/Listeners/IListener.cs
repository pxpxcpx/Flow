using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Listeners;

public interface IListener
{
    string Name { get; }
    
    List<ICondition> Conditions { get; }
    
    IObservable<EventMessage> EventStream { get; }

    void Initialize();
    
    Task StartAsync();
    
    Task StopAsync();
    
    ProcessorStatus Status { get; }
}