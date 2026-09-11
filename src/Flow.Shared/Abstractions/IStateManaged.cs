namespace Flow.Shared.Abstractions;

public interface IStateManaged<out TState>
{
    TState State { get; }
}