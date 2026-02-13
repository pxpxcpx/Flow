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
    /// Initialize the graph.
    /// </summary>
    void Initialize();
}
