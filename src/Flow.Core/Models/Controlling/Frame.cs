using Flow.Core.Abstractions.Enums;
using Flow.Core.Abstractions.Interfaces;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using ExecutionContext = Flow.Core.Models.Context.ExecutionContext;

namespace Flow.Core.Models.Controlling;

/// <summary>
/// Record represents a frame during runtime.
/// Frames correspond one-to-one with <see cref="Function"/>, includes the runtime information of this function. 
/// </summary>
/// <remarks>
/// Frame couldn't be reset.
/// </remarks>
internal sealed record Frame : IStateMachine<FrameStates, FrameEvents>
{
    /// <summary>
    /// Related <see cref="IFunction"/>.
    /// </summary>
    public IFunction Function { get; init; }

    /// <summary>
    /// Parent of this frame, null when this frame itself is the root.
    /// </summary>
    public Frame? Parent { get; init; }

    /// <summary>
    /// Pointer to the current node.
    /// </summary>
    public Guid Current { get; }

    /// <summary>
    /// Indicates the id of the node which this frame should return to after the execution.
    /// </summary>
    public Guid? ReturnTo { get; set; }

    /// <summary>
    /// Pending count.
    /// </summary>
    public int Pending { get; }

    /// <summary>
    /// Execution context of <see cref="Function"/>.
    /// </summary>
    public ExecutionContext ExecutionContext { get; init; }

    /// <inheritdoc />
    public FrameStates State { get; private set; }

    /// <inheritdoc />
    public FrameStates PreviousState { get; private set; }

    /// <summary>
    /// Initialize a new Frame.
    /// </summary>
    /// <param name="function">Related <see cref="Flow.Core.Models.Context.Function"/>.</param>
    /// <param name="current">Guid pointer to current node.</param>
    /// <param name="executionContext"><see cref="ExecutionContext"/></param>
    internal Frame(Function function, Guid current, ExecutionContext executionContext)
    {
        State = FrameStates.None;
        PreviousState = FrameStates.None;

        Function = function;
        Current = current;
        ExecutionContext = executionContext;
        Pending = 0;

        Fire(FrameEvents.Initialize);
    }

    /// <inheritdoc />
    public bool Fire(FrameEvents @event)
    {
        var pr = PreviousState;

        FrameStates? newState = (pr, @event) switch
        {
            (_, FrameEvents.None) => pr, // no modification
            (FrameStates.None, FrameEvents.Initialize) => FrameStates.Ready,
            (FrameStates.Ready, FrameEvents.MoveNext) => FrameStates.Suspended,
            (FrameStates.Ready, FrameEvents.Pause) => FrameStates.Suspended,
            (FrameStates.Suspended, FrameEvents.Pause) => FrameStates.Suspended,
            (FrameStates.Suspended, FrameEvents.Continue) => FrameStates.Ready,
            // Only those that are in a Suspended state will receive the Resume event.
            // Otherwise, there could be other possibilities for the previous state.
            (FrameStates.Suspended, FrameEvents.Resume) => PreviousState, 
            (_, FrameEvents.Terminate) => FrameStates.Terminated,
            _ => null
        };

        if (newState is null) 
            return false;
        
        // Only legal combinations can change these states.
        PreviousState = State;
        State = newState.Value;
        return true;
    }
}