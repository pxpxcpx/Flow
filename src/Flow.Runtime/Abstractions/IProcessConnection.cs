using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

public interface IProcessConnection
{
    NodePosition From { get; set; }

    NodePosition To { get; set; }
}
