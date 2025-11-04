using Flow.Automation.Decisioning.Abstractions;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.BuiltIn.ControlStatements;

public class IfStatement : INode, IControlStatement, IInstanceRequired
{
    public NodeMetadata Metadata { get; }
    
    public object? RequiredInstance { get; set; }

    public Type InstanceType => typeof(ISpecification);
    
    public object? Instance { get; set; }
    
    public ParameterMetadata[]? InputVariableMetadata => null;

    public ParameterMetadata[]? OutputVariableMetadata => null;
    
    public object?[]? Inputs { get; init; }
    
    public object?[]? Outputs { get; init; }
    
    public Result Result { get; }
    
    public int ReturnIndex { get; set; }
    
    public void Execute()
    {
        throw new NotImplementedException();
    }
}
