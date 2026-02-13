using Flow.Shared.Abstractions;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Treat a function as a node.
/// A special kind of node. (That's also why this file is not under Flow.Shared.Abstractions) 
/// </summary>
public interface IFunctionNode : INode, IExecutable, IFunction;