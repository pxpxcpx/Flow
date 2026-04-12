using Flow.Core.Abstractions;

namespace Flow.Core.Models.Positioning;

/// <summary>
/// Connection between two <see cref="VariablePort">value points</see> coming from two different nodes
/// </summary>
public record struct VariableConnection(VariablePort Source, VariablePort Target, Type SourceType, Type TargetType)
    : IVariableConnection
{
    public required VariablePort Source { get; set; } = Source;

    public required VariablePort Target { get; set; } = Target;

    public required Type SourceType { get; set; } = SourceType;
    
    public required Type TargetType { get; set; } = TargetType;
}