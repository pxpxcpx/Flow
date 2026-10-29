namespace Flow.Shared.Abstractions;

public interface IStateManaged<out TState>
{
    /// <summary>
    /// Represents the current state of the object.
    /// </summary>
    TState State { get; }
}