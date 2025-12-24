using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.BuiltIn.ControlStatements;

public class WhileStatement : IControlStatement
{
    public int ReturnIndex { get; set; }
    
    public ProcessPointMetadata[] ProcessPointMetadata { get; }
}