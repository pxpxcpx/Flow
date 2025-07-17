using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Abstractions;

/// <summary>
/// 表示定义节点、连接和入口逻辑序列的脚本
/// </summary>
/// <remarks>
/// 该接口提供脚本的结构，包括其唯一标识符、起始节点、以及各种联系。
/// 此接口的实现应定义 <see cref="Initialize"/>，该方法用作执行脚本的入口点。
/// </remarks>
public interface IScript
{
    /// <summary>
    /// 脚本的Id
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// 起始节点
    /// </summary>
    INode StartNode { get; set; }

    /// <summary>
    /// 节点的集合
    /// </summary>
    IEnumerable<INode> Nodes { get; set; }

    /// <summary>
    /// 执行流程的连接集合
    /// </summary>
    IEnumerable<IProcessConnection> ProcessConnection { get; set; }

    /// <summary>
    /// 数据传输的连接集合
    /// </summary>
    IEnumerable<IVariableConnection> VariableConnections { get; set; }

    /// <summary>
    /// 初始化脚本需要的资源或状态
    /// </summary>
    void Initialize();
}
