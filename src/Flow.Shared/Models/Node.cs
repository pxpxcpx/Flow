using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Shared.Models;

/// <summary>
/// Base class for common implementations of <see cref="INode"/>.
/// Also provides node utils.
/// </summary>
public abstract class Node : INode, IEquatable<INode>
{
    /// <inheritdoc />
    public NodeMetadata Metadata { get; init; }

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }
    
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
    public virtual Result? Result { get; protected set; }

    protected Node(): this(NodeMetadata.Empty, [], [])
    {
    }
    
    protected Node(
        NodeMetadata metadata, ParameterMetadata[]? inputVariableMetadata, ParameterMetadata[]? outputVariableMetadata)
    {
        RuntimeId = Guid.NewGuid();
        Status = NodeStatus.Ready;
        
        Metadata = metadata;
        InputVariableMetadata = inputVariableMetadata;
        OutputVariableMetadata = outputVariableMetadata;
    }
    
    private static bool IsNullOrIndexOutOfRange<T>(T?[]? array, int index)
        => array == null || index < 0 || index >= array.Length;

    public static object? GetInput(INode node, int index)
        => IsNullOrIndexOutOfRange(node.Inputs, index) ? null : node.Inputs![index];

    public static bool GetInput<T>(INode node, int index, out T? value)
    {
        var o = GetInput(node, index);

        try
        {
            value = (T?)o;
            return true;
        }
        catch (InvalidCastException)
        {
            value = default;
        }
        
        return false;
    }
    
    public static object? GetOutput(INode node, int index)
        => IsNullOrIndexOutOfRange(node.Outputs, index) ? null : node.Outputs![index];

    public static bool GetOutput<T>(INode node, int index, out T? value)
    {
        var o = GetOutput(node, index);
        
        try
        {
            value = (T?)o;
            return true;
        }
        catch (InvalidCastException)
        {
            value = default;
        }
        
        return false;
    }

    /// <summary>
    /// Pass a parameter to the input of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="index">Input parameter's index.</param>
    /// <param name="value">Value to pass.</param>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>True if successful, otherwise, false.</returns>
    public static bool Assign<TValue>(INode node, int index, TValue? value)
    {
        if (IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) || IsNullOrIndexOutOfRange(node.Inputs, index))
            return false;

        var md = node.InputVariableMetadata![index];
        var type = md.Type;
        if (typeof(TValue) != type)
            return false;

        node.Inputs![index] = value;
        return true;
    }

    /// <summary>
    /// Pass a parameter to the input of the node.
    /// </summary>
    /// <remarks>Will NOT check the type of the value.</remarks>
    /// <param name="node"></param>
    /// <param name="index">Input parameter's index.</param>
    /// <param name="value">Value to pass.</param>
    /// <returns>True if successful, otherwise, false.</returns>
    public static bool Assign(INode node, int index, object? value)
    {
        if (IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) || IsNullOrIndexOutOfRange(node.Inputs, index))
            return false;

        node.Inputs![index] = value;
        return true;
    }

    public static bool SetToDefaultValue(INode node, int index)
    {
        if (IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) || IsNullOrIndexOutOfRange(node.Inputs, index))
            return false;

        var md = node.InputVariableMetadata![index];
        if (md.DefaultValue is not { } dv)
            return false;

        node.Inputs![index] = dv;
        return true;
    }

    public static IEnumerable<ParameterMetadata> GetUnfilledRequiredValues(INode node)
    {
        if (node.InputVariableMetadata is null || node.Inputs is null)
            yield break;

        for (var i = 0; i < node.InputVariableMetadata.Length; i++)
        {
            var metadata = node.InputVariableMetadata[i];

            if (!metadata.IsRequired)
                continue;

            if (node.Inputs[i] is null && metadata.DefaultValue is null)
                yield return metadata;
        }
    }

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

    public static void MarkAs(INode node, NodeStatus status)
        => node.Status = status;

#if DEBUG
    public override string ToString()
    {
        return 
            $"""
            Type: {this.GetType().Name}", 
            RuntimeId: {RuntimeId},
            Metadata:{Metadata}
            """;
    }
#endif

    private static bool Equals(INode node, INode? other)
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
    public virtual INode? Clone() 
        => throw new NotImplementedException("The base class did not implement the Clone() method.");

    public override bool Equals(object? obj)
    {
        if (obj is not INode other)
            return false;

        return Equals(this, other);
    }

    public bool Equals(INode? other)
        => Equals(this, other);

    public override int GetHashCode() 
        => HashCode.Combine(Metadata, RuntimeId, InputVariableMetadata, OutputVariableMetadata);
}