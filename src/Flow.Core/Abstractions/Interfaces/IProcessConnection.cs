using Flow.Core.Models.Positioning;

namespace Flow.Core.Abstractions.Interfaces;

public interface IProcessConnection
{
    NodePort Source { get; set; }

    NodePort Target { get; set; }
}
