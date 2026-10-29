using System.Diagnostics;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;
using Flow.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Shared.Models;

/// <summary>
/// Base class for common implementations of <see cref="INode"/>.
/// Also provides node utils.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public abstract class Node : INode, IEquatable<INode>, ICloneable
{
    /// <inheritdoc />
    public NodeMetadata Metadata { get; init; }

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    public NodeStates State { get; internal set; }

    /// <inheritdoc />
    public NodeStates PreviousState { get; internal set; }

    public bool IsCompleted => State.ExtractFieldIn(NodeStates.LifecycleMask) == (int)NodeStates.Finished;

    public bool IsCompletedSuccessfully =>
        IsCompleted && State.ExtractFieldIn(NodeStates.ResultMask) == (int)NodeStates.Successful;

    public bool IsInitialized => State.ExtractFieldIn(NodeStates.LifecycleMask) != (int)NodeStates.Ready;

    /// <summary>
    /// Services required.
    /// </summary>
    public object?[]? Services { get; init; }

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata { get; init; }

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata { get; init; }

    /// <inheritdoc />
    public virtual object?[]? Inputs { get; set; }

    /// <inheritdoc />
    public virtual object?[]? Outputs { get; set; }

    /// <inheritdoc />
    public virtual VoidResult? Result { get; protected set; }

    protected Node() : this(NodeMetadata.Empty, [], [])
    {
    }

    protected Node(
        NodeMetadata metadata, ParameterMetadata[]? inputVariableMetadata, ParameterMetadata[]? outputVariableMetadata)
    {
        IsEnabled = true;
        RuntimeId = Guid.NewGuid();
        Fire(NodeEvents.Initialize);

        Metadata = metadata;
        InputVariableMetadata = inputVariableMetadata;
        OutputVariableMetadata = outputVariableMetadata;
    }

    internal static bool IsNullOrIndexOutOfRange<T>(T?[]? array, int index)
        => array == null || index < 0 || index >= array.Length;

    // TODO: Resolve the services before initializing.
    /// <summary>
    /// Attempts to resolve and assign all required services from the specified service provider.
    /// </summary>
    /// <remarks>If any required service cannot be resolved, the method returns false and no further services
    /// are assigned. The method does not throw exceptions for missing services.</remarks>
    /// <param name="serviceSource">The service provider used to resolve the required service instances. Cannot be null.</param>
    /// <returns>true if all required services are successfully resolved and assigned; otherwise, false.</returns>
    public bool GetRequiredServices(IServiceProvider serviceSource)
    {
        if (Metadata.RequiredServices is null || !Metadata.RequiredServices.Any())
            return false;

        var s = Metadata.RequiredServices.ToArray();

        for (var i = 0; i < s.Length; i++)
        {
            var t = s.ElementAt(i);
            try
            {
                Services?[i] = serviceSource.GetRequiredService(t);
            }
            catch
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Find the state by related binary/int code.
    /// </summary>
    public static NodeStates ExtractNodeState(int code)
        => code switch
        {
            2 => NodeStates.Ready,
            4 => NodeStates.Running,
            8 => NodeStates.Suspended,
            16 => NodeStates.Finished,
            32 => NodeStates.Unspecified,
            64 => NodeStates.Faulted,
            128 => NodeStates.Successful,
            _ => NodeStates.None
        };

    /// <summary>
    /// Find the event by related binary/int code.
    /// </summary>
    public static NodeEvents ExtractNodeEvent(int code)
        => code switch
        {
            1 => NodeEvents.Initialize,
            2 => NodeEvents.Start,
            3 => NodeEvents.Suspend,
            4 => NodeEvents.Pause,
            5 => NodeEvents.Resume,
            6 => NodeEvents.Cancel,
            7 => NodeEvents.Complete,
            8 => NodeEvents.Reset,
            16 => NodeEvents.Unspecified,
            32 => NodeEvents.Success,
            48 => NodeEvents.Fail,
            _ => NodeEvents.None
        };

    /// <summary>
    /// Default state transition of common nodes.
    /// </summary>
    /// <param name="pr">Current state(before the transition).</param>
    /// <param name="event">Event received.</param>
    /// <param name="newLifecycleState">New state of node lifecycle.</param>
    /// <param name="newResultState">New state of processing result.</param>
    /// <returns></returns>
    public static void DefaultStateTransform(
        NodeStates pr, NodeEvents @event, out NodeStates? newLifecycleState, out NodeStates? newResultState)
    {
        var lifeCycleEvent = @event.ExtractFieldIn(NodeEvents.LifecycleMask, ExtractNodeEvent);
        var procEvent = @event.ExtractFieldIn(NodeEvents.ResultMask, ExtractNodeEvent);
        var preLifecycleState = pr.ExtractFieldIn(NodeStates.LifecycleMask, ExtractNodeState);
        var preProcState = pr.ExtractFieldIn(NodeStates.ResultMask, ExtractNodeState);

        if (lifeCycleEvent == NodeEvents.Reset)
        {
            newLifecycleState = NodeStates.Ready;
            newResultState = NodeStates.Unspecified;
            return;
        }

        NodeStates? l = (preLifecycleState, lifeCycleEvent) switch
        {
            // * --None--> *
            (_, NodeEvents.None) => preLifecycleState, // no modification on lifecycle state
            // Idle -> *
            (NodeStates.None, NodeEvents.Initialize) => NodeStates.Ready,
            // Ready -> *
            (NodeStates.Ready, NodeEvents.Start) => NodeStates.Running,
            (NodeStates.Ready, NodeEvents.Suspend) => NodeStates.Suspended,
            // Running -> *
            (NodeStates.Running, NodeEvents.Pause) => NodeStates.Suspended,
            (NodeStates.Running, NodeEvents.Suspend) => NodeStates.Suspended,
            (NodeStates.Running, NodeEvents.Cancel) => NodeStates.Finished,
            (NodeStates.Running, NodeEvents.Complete) => NodeStates.Finished,
            // Suspended -> *
            (NodeStates.Suspended, NodeEvents.Resume) => NodeStates.Running,
            (NodeStates.Suspended, NodeEvents.Cancel) => NodeStates.Finished,
            _ => null
        };

        NodeStates? r = procEvent switch
        {
            NodeEvents.None => preProcState, // no modification on process result state
            NodeEvents.Unspecified => NodeStates.Unspecified,
            NodeEvents.Success => NodeStates.Successful,
            NodeEvents.Fail => NodeStates.Faulted,
            _ => null
        };

        newLifecycleState = l;
        newResultState = r;
    }

    /// <inheritdoc />
    public bool Fire(NodeEvents @event)
    {
        var pr = State;
        DefaultStateTransform(pr, @event, out var l, out var r);

        if (l is null || r is null)
            return false;

        State = l.Value | r.Value;
        PreviousState = State;
        return true;
    }

    public override string ToString()
        => $"Type = {GetType().Name}, RuntimeId = {RuntimeId}, Name = {Metadata.Identifier}";

    private static bool Equals(Node node, INode? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(node, other))
            return true;

        return other.Metadata == node.Metadata && node.RuntimeId == other.RuntimeId &&
               node.InputVariableMetadata == other.InputVariableMetadata &&
               node.OutputVariableMetadata == other.OutputVariableMetadata;
    }

    /// <summary>
    /// Clone the node itself.
    /// </summary>
    /// <remarks>
    /// Due to <see cref="Node"/> is an abstract class,
    /// this method must be implemented manually if the node want to support cloning.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">
    /// <see cref="Node"/> is an abstract class, throws when calling without overriding.
    /// </exception>
    public virtual INode Clone()
        // Not a to-do:
        => throw new NotImplementedException("The base class did not implement the Clone() method.");

    object ICloneable.Clone()
        => Clone();

    public override bool Equals(object? obj)
    {
        if (obj is not INode other)
            return false;

        return Equals(this, other);
    }

    /// <inheritdoc />
    public bool Equals(INode? other)
        => Equals(this, other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Metadata, RuntimeId, InputVariableMetadata, OutputVariableMetadata);
}