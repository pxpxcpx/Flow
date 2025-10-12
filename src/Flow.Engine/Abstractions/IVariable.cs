using Flow.SDK.Plugins;

namespace Flow.Engine.Abstractions;

/// <summary>
/// Represents an argument or an output in the node.
/// </summary>
public interface IVariable : IRecognizable
{
    /// <summary>
    /// Indicates input or output.
    /// </summary>
    VariableTypes VariableType { get; }
    
    /// <summary>
    /// Type of the <see cref="Value"/>
    /// </summary>
    Type Type { get; }
    
    /// <summary>
    /// Value that exists in the form of object. Can be null.
    /// </summary>
    object? Value { get; }
}