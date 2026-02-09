using Flow.Runtime.ContextManager;
using Flow.Shared.Abstractions;
using Flow.Shared.Models;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Represents a script that defines a logical sequence of nodes, connections, and portals
/// </summary>
/// <remarks>
/// This interface provides the structure of the script,
/// including its unique identifier, starting node, and various connections.
/// </remarks>
public interface IScript : IFunction
{
    /// <summary>
    /// Script's unique identifier
    /// </summary>
    Guid RuntimeId { get; init; }

    /// <summary>
    /// Plugins that the script depends on.
    /// </summary>
    Dependency[] Dependencies { get; init; }
    
    /// <summary>
    /// Entry point of the script, which is the starting node.
    /// </summary>
    /// <remarks>Can be null if the script is empty.</remarks>
    INode? Entry { get; set; }

    /// <summary>
    /// Contains nodes and their GUID.
    /// The internal graph of the script.
    /// </summary>
    Dictionary<Guid, INode> Nodes { get; }

    // TODO: Standalone graph data structure
    /// <summary>
    /// Collection of process controlling connections in the script
    /// </summary>
    HashSet<IProcessConnection> ProcessConnections { get; set; }

    /// <summary>
    /// Collection of variable connections in the script
    /// </summary>
    HashSet<IVariableConnection> VariableConnections { get; set; }
    
    /// <summary>
    /// Connection between <see cref="ContextItem"/> items
    /// and <see cref="IInstanceRequired"/> in the script.
    /// </summary>
    HashSet<InstanceConnection> InstanceConnections { get; set; }

    /// <summary>
    /// Initialize the graph.
    /// </summary>
    void Initialize();
}
