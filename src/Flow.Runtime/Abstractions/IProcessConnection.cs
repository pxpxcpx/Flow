using Flow.Runtime.Models.Positioning;

namespace Flow.Runtime.Abstractions;

public interface IProcessConnection
{
    NodePosition Source { get; set; }

    NodePosition Target { get; set; }
}
