using Flow.Core.Abstractions;

namespace Flow.Core.Models.Positioning;

/// <summary>
/// Connection between two <see cref="VariablePosition">value points</see> coming from two different nodes
/// </summary>
public record struct VariableConnection(VariablePosition Source, VariablePosition Target, Type SourceType, Type TargetType)
    : IVariableConnection
{
    public required VariablePosition Source { get; set; } = Source;

    public required VariablePosition Target { get; set; } = Target;

    public required Type SourceType { get; set; } = SourceType;
    
    public required Type TargetType { get; set; } = TargetType;
}