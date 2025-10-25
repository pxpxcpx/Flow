using Flow.Shared.Metadata;

namespace Flow.Shared.Abstractions;

/// <summary>
/// The interface of a node to be executed.
/// </summary>
public interface INode : IExecutable
{
    /// <summary>
    /// Metadata of the node. Includes name, description, GUID of this node. 
    /// </summary>
    NodeMetadata Metadata { get; }
    
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
    object?[]? Inputs { get; init; }
    
    /// <summary>
    /// Data of the outputs. Can be null if there's no output.
    /// </summary>
    object?[]? Outputs { get; init; }
    
    // Result Result { get; }
}
