namespace Flow.Runtime.Abstractions;

/// <summary>
/// Represents a script that defines a logical sequence of nodes, connections, and portals
/// </summary>
/// <remarks>
/// This interface provides the structure of the script,
/// including its unique identifier, starting node, and various connections.
/// </remarks>
public interface IScript
{
    /// <summary>
    /// Script's unique identifier
    /// </summary>
    Guid RuntimeId { get; init; }

    /// <summary>
    /// Entry point of the script, which is the starting node.
    /// </summary>
    /// <remarks>Can be null if the script is empty.</remarks>
    IRuntimeNode? Entry { get; set; }

    /// <summary>
    /// Contains nodes and their GUID.
    /// The internal graph of the script.
    /// </summary>
    List<IRuntimeNode> Nodes { get; }

    /// <summary>
    /// Collection of process controlling connections in the script
    /// </summary>
    List<IProcessConnection> ProcessConnection { get; set; }

    /// <summary>
    /// Collection of variable connections in the script
    /// </summary>
    List<IVariableConnection> VariableConnections { get; set; }

    void InitializeGraph();
}
