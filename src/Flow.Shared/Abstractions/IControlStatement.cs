using Flow.Shared.Metadata;

namespace Flow.Shared.Abstractions;

public interface IControlStatement
{
    int ReturnIndex { get; set; }
    
    ProcessPointMetadata[] ProcessPointMetadata { get; }
}