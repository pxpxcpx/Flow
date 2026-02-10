using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

public interface IVariableConnection
{
    VariablePosition Source { get; set; }

    VariablePosition Target { get; set; }

    Type SourceType { get; set; }
    
    Type TargetType { get; set; }
}
