using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Listeners;

public abstract class Listener : IListener
{
    private readonly Subject<ListenerEventMessage> _subject = new();
    
    public string Name { get; }
    
    public Dictionary<Guid, ICondition> Conditions { get; }
    
    public IObservable<ListenerEventMessage> EventStream => _subject.AsObservable();
    
    public ProcessorStatus Status { get; }

    public abstract void Initialize();

    public abstract Task StartAsync();

    public abstract Task StopAsync();

    protected abstract ListenerEventMessage GenerateEventMessage();
    
    protected virtual bool CheckCondition<TObj>(ICondition condition, TObj? obj = default)
    {
        if (condition is IParamCondition<TObj> ps){
            return ps.Evaluate(obj);}
        
        // TODO: 我们需要一种包含了GUID和原始信息的载体(EventMessage'), 这样既可以让router知道handler是谁, 也能给handler传原始消息
        // Example below:
        // _subject.OnNext(GenerateEventMessage());
        return condition.Result;
    }
}