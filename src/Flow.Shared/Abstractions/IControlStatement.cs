using Flow.Shared.Metadata;

namespace Flow.Shared.Abstractions;

public interface IControlStatement
{
    int ReturnedPort { get; set; }
    
    ProcessPointMetadata[] ProcessPointMetadata { get; }
}