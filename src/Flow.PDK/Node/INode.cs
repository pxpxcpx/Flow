namespace Flow.PDK.Node;

public interface INode : IExecutable, IRecognizable
{
    Guid Id { get; init; }
}
