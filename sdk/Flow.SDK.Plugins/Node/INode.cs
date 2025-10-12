namespace Flow.SDK.Plugins.Node;

/// <summary>
/// The interface of a node to be executed.
/// </summary>
public interface INode : IExecutable, IRecognizable
{
    /// <summary>
    /// ID of the node, used to distinguish between different nodes.
    /// Determined during development, not changed at runtime.
    /// </summary>
    /// <remarks> Note the distinction from the runtime GUID. </remarks>
    Guid Guid { get; }
    
    /// <summary>
    /// A dictionary that includes parameters and their corresponding types.
    /// </summary>
    Dictionary<string, Type> ParamTypes { get; }
    
    object?[] Inputs { get; set; }
    
    object[]? Outputs { get; set; }
    
    /// <summary>
    /// Set the value of the node.
    /// </summary>
    /// <param name="param">Parameter name.</param>
    /// <param name="value">Value to be set.</param>
    /// <typeparam name="T">Type of the parameter.</typeparam>
    void SetValue<T>(string param, T value);
}
