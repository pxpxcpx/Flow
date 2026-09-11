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
public abstract class Node : INode, IEquatable<INode>, ICloneable, IStateMachine<NodeStates, NodeEvents>
{
    /// <inheritdoc />
    public NodeMetadata Metadata { get; init; }

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }

    /// <inheritdoc />
    public NodeStates State { get; private set; }

    /// <inheritdoc />
    public NodeStates PreviousState { get; private set; }

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
        Status = NodeStatus.Ready;

        Metadata = metadata;
        InputVariableMetadata = inputVariableMetadata;
        OutputVariableMetadata = outputVariableMetadata;
    }

    internal static bool IsNullOrIndexOutOfRange<T>(T?[]? array, int index)
        => array == null || index < 0 || index >= array.Length;

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
    /// Find the event by related binary/int code.
    /// </summary>
    private static NodeEvents ExtractEvent(int code)
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
    /// Find the state by related binary/int code.
    /// </summary>
    private static NodeStates ExtractState(int code)
        => code switch
        {
            1 => NodeStates.Idle,
            2 => NodeStates.Ready,
            4 => NodeStates.Running,
            8 => NodeStates.Suspended,
            16 => NodeStates.Finished,
            32 => NodeStates.Unspecified,
            64 => NodeStates.Faulted,
            128 => NodeStates.Successful,
            _ => NodeStates.None
        };

    /// <inheritdoc />
    public bool Fire(NodeEvents @event)
    {
        PreviousState = State;
        var pr = State;

        var lifeCycleEvent = @event.ExtractFieldIn(NodeEvents.LifecycleMask, ExtractEvent);
        var procEvent = @event.ExtractFieldIn(NodeEvents.ResultMask, ExtractEvent);
        var lifecycleState = pr.ExtractFieldIn(NodeStates.LifecycleMask, ExtractState);

        if (lifeCycleEvent == NodeEvents.Reset)
        {
            State = lifecycleState.HasFlag(NodeStates.Idle)
                ? NodeStates.Idle | NodeStates.Unspecified
                : NodeStates.Ready | NodeStates.Unspecified;
            return true;
        }

        NodeStates? l = (lifecycleState, lifeCycleEvent) switch
        {
            // * --None--> *
            (_, NodeEvents.None) => pr,
            // Idle -> *
            (NodeStates.Idle, NodeEvents.Initialize) => NodeStates.Ready,
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

        var r = procEvent switch
        {
            NodeEvents.Unspecified => NodeStates.Unspecified,
            NodeEvents.Success => NodeStates.Successful,
            NodeEvents.Fail => NodeStates.Faulted,
            _ => NodeStates.Unspecified
        };

        if (l is null) return false;
        State = l.Value | r;
        return true;
    }

    public override string ToString()
        => $"Type: {GetType().Name}, RuntimeId: {RuntimeId}, Metadata:{Metadata}";

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