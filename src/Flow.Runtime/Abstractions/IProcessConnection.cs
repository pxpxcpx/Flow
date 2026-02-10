using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

public interface IProcessConnection
{
    NodePosition Source { get; set; }

    NodePosition Target { get; set; }
}
