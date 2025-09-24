namespace Flow.SDK.Plugins.Node;

public interface INode : IExecutable, IRecognizable
{
    Guid Id { get; init; }
}
