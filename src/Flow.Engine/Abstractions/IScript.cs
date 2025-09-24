using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.SDK.Plugins.Node.Internal;

namespace Flow.Engine.Abstractions;

/// <summary>
/// Represents a script that defines a logical sequence of nodes, connections, and portals
/// </summary>
/// <remarks>
/// 该接口提供脚本的结构，包括其唯一标识符、起始节点、以及各种联系。
/// This interface provides the structure of the script, including its unique identifier, starting node, and various connections.
/// </remarks>
public interface IScript
{
    /// <summary>
    /// Script's unique identifier
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Entry point of the script, which is the starting node
    /// (Maybe removed in the next version)
    /// </summary>
    IInternalNode Entry { get; set; }

    Dictionary<Guid, IInternalNode> Graph { get; }
    
    /// <summary>
    /// Collection of nodes in the script
    /// </summary>
    IEnumerable<IInternalNode> Nodes { get; set; }

    /// <summary>
    /// Collection of process controlling connections in the script
    /// </summary>
    IEnumerable<IProcessConnection> ProcessConnection { get; set; }

    /// <summary>
    /// Collection of variable connections in the script
    /// </summary>
    IEnumerable<IVariableConnection> VariableConnections { get; set; }

    void InitializeGraph();
}
