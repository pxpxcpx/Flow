using Flow.Core.Models.Positioning;

namespace Flow.Core.Abstractions.Interfaces;

public interface IVariableConnection
{
    VariablePort Source { get; set; }

    VariablePort Target { get; set; }

    Type SourceType { get; set; }
    
    Type TargetType { get; set; }
}
