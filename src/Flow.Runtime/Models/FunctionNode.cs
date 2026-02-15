using Flow.Runtime.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Runtime.Models;

public class FunctionNode : Function, IFunctionNode
{
    public NodeMetadata Metadata { get; }
    public Guid RuntimeId { get; }
    public NodeStatus Status { get; set; }
    public ParameterMetadata[]? InputVariableMetadata { get; }
    public ParameterMetadata[]? OutputVariableMetadata { get; }
    public object?[]? Inputs { get; init; }
    public object?[]? Outputs { get; init; }
    public Result? Result { get; }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}