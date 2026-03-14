using Flow.Core.Models.Positioning;

namespace Flow.Core.Abstractions;

public interface IProcessConnection
{
    NodePosition Source { get; set; }

    NodePosition Target { get; set; }
}
