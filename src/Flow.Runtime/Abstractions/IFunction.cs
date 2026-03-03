using Flow.Runtime.ContextManager;
using Flow.Runtime.Models;
using Flow.Runtime.Models.Positioning;
using Flow.Shared.Abstractions;

namespace Flow.Runtime.Abstractions;

public interface IFunction : INode
{
    /// <summary>
    /// Entry point of the script, which is the starting node.
    /// </summary>
    IExecutableNode Entrance { get; }
    
    /// <summary>
    /// Exit of the function, exit when any is executed.
    /// </summary>
    IExecutableNode Exit { get; }

    /// <summary>
    /// Contains nodes and their GUID.
    /// The internal graph of the script.
    /// </summary>
    Dictionary<Guid, INode> Nodes { get; }

    // TODO: Standalone graph data structure
    /// <summary>
    /// Collection of process controlling connections in the script
    /// </summary>
    HashSet<ProcessConnection> ProcessConnections { get; set; }

    /// <summary>
    /// Collection of variable connections in the script
    /// </summary>
    HashSet<VariableConnection> VariableConnections { get; set; }
    
    /// <summary>
    /// Connection between <see cref="ContextItem"/> items
    /// and <see cref="IInstanceRequired"/> in the script.
    /// </summary>
    HashSet<InstanceConnection> InstanceConnections { get; set; }
}