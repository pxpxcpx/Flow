using Flow.Core.Models.Positioning;

namespace Flow.Core.Abstractions;

public interface IProcessConnection
{
    NodePort Source { get; set; }

    NodePort Target { get; set; }
}
