using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Triggers;

public interface IListener
{
    string Name { get; }
    
    List<ISpecification> Specifications { get; }
    
    IObservable<EventMessage> EventStream { get; }

    void Initialize();
    
    Task StartAsync();
    
    Task StopAsync();
    
    ProcessorStatus Status { get; }
}