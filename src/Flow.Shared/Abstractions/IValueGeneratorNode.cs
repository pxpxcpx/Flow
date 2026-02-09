using Flow.Shared.Metadata;

namespace Flow.Shared.Abstractions;

/// <summary>
/// Interface for a value generator.
/// Value Generator <b>CAN NOT</b> have process connections, and only generate one value every time.
/// </summary>
public interface IValueGeneratorNode : IExecutableNode
{
    /// <summary>
    /// Metadata of the output.
    /// </summary>
    new ParameterMetadata OutputVariableMetadata { get; }
    
    /// <summary>
    /// Generated value.
    /// </summary>
    new object? Outputs { get; init; }
}