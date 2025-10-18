using Flow.SDK.Plugins.Metadata;

namespace Flow.SDK.Plugins.Node;

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
    ParameterMetadata[] InputVariableMetadata { get; }
    
    /// <summary>
    /// Metadata of the output variables.
    /// </summary>
    ParameterMetadata[] OutputVariableMetadata { get; }
    
    /// <summary>
    /// Data of the Inputs (arguments).
    /// </summary>
    object?[] Inputs { get; init; }
    
    /// <summary>
    /// Data of the outputs.
    /// </summary>
    object[]? Outputs { get; init; }
}
