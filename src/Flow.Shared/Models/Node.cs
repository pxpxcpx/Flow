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

    public bool IsInitialized => State.ExtractFieldIn(NodeStates.LifecycleMask) != (int)NodeStates.Idle;

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

    /// <inheritdoc />
    public bool Fire(NodeEvents @event)
    {
        PreviousState = State;
        var pr = State;
        // TODO
        State = NodeExtensions.DefaultStateTransform(pr, @event);
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
        // Not a todo:
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