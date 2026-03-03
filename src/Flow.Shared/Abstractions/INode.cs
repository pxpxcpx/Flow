using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Shared.Abstractions;

/// <summary>
/// An interface providing basic node composition and runtime information.
/// <remarks>
/// To implement the node, simply inherit from <see cref="IExecutableNode"/> / <see cref="IAsyncExecutableNode"/>.
/// </remarks>
/// </summary>
public interface INode
{
    /// <summary>
    /// Metadata of the node. Includes name, description, GUID of this node. 
    /// </summary>
    NodeMetadata Metadata { get; }
    
    /// <summary>
    /// ID during the runtime.
    /// </summary>
    /// <remarks>Note the distinction from the <see cref="NodeMetadata.Id"/></remarks>
    Guid RuntimeId { get; }
    
    /// <summary>
    /// Node status during the runtime.
    /// </summary>
    NodeStatus Status { get; set; }
    
    /// <summary>
    /// Metadata of the input variables.
    /// </summary>
    ParameterMetadata[]? InputVariableMetadata { get; }
    
    /// <summary>
    /// Metadata of the output variables.
    /// </summary>
    ParameterMetadata[]? OutputVariableMetadata { get; }
    
    /// <summary>
    /// Data of the Inputs (arguments). Can be null if there's no input.
    /// </summary>
    object?[]? Inputs { get; }
    
    /// <summary>
    /// Data of the outputs. Can be null if there's no output.
    /// </summary>
    object?[]? Outputs { get; }
    
    /// <summary>
    /// Used as an alternative to throwing exceptions.
    /// </summary>
    Result? Result { get; }
}
