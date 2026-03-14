using Flow.Shared.Models;

namespace Flow.Core.Abstractions;

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
    /// Plugins that the script depends on.
    /// </summary>
    Dependency[] Dependencies { get; init; }

    /// <summary>
    /// Initialize the graph.
    /// </summary>
    void Initialize();
}
