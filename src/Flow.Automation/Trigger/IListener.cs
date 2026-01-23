using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Trigger;

public interface IListener
{
    string Name { get; }
    
    IObservable<EventMessage> EventStream { get; }

    Task StartAsync();
    
    Task StopAsync();
    
    ProcessorStatus Status { get; }
}