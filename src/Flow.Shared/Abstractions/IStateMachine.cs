namespace Flow.Shared.Abstractions;

public interface IStateMachine<out TState, in TEvent> : IStateManaged<TState>
{
    TState PreviousState { get; }
    
    bool Fire(TEvent @event);
}