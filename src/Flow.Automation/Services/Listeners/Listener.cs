using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Listeners;

public abstract class Listener : IListener
{
    private readonly Subject<EventMessage> _subject = new();
    
    public string Name { get; }
    
    public Dictionary<Guid, ICondition> Conditions { get; }
    
    public IObservable<EventMessage> EventStream => _subject.AsObservable();
    
    public ProcessorStatus Status { get; }

    public abstract void Initialize();

    public abstract Task StartAsync();

    public abstract Task StopAsync();

    protected abstract EventMessage GenerateEventMessage();
    
    protected virtual bool CheckCondition<TObj>(ICondition condition, TObj? obj = default)
    {
        if (condition is IParamCondition<TObj> ps){
            return ps.Evaluate(obj);}
        
        return condition.Result;
    }
}