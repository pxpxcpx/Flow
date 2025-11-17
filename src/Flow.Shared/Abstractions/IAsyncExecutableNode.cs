namespace Flow.Shared.Abstractions;

/// <summary>
/// A node can be executed asynchronously.
/// </summary>
public interface IAsyncExecutableNode : INode, IAsyncExecutable;