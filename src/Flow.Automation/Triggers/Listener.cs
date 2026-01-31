using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Triggers;

public class Listener<TEvent>: IListener
{
    public string Name { get; }
    public List<ISpecification> Specifications { get; }
    public IObservable<EventMessage> EventStream { get; }
    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync()
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        throw new NotImplementedException();
    }

    public ProcessorStatus Status { get; }
}