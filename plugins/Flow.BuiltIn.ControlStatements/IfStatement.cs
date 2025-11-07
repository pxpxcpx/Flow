using Flow.Automation.Decisioning.Abstractions;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.BuiltIn.ControlStatements;

/// <summary>
/// “If” conditional statement node.
/// </summary>
public class IfStatement : INode, IControlStatement, IInstanceRequired
{
    private static readonly NodeMetadata NodeMetadata = new()
    {
        Id = new Guid("7AE6EB1F-5B2C-4495-99D1-855CCF8B6FD0"),
        Name = "If statement",
        Description = "Used as a conditional statement node."
    };
    
    /// <inheritdoc />
    public NodeMetadata Metadata => NodeMetadata;
    
    /// <inheritdoc />
    public Type InstanceType => typeof(ISpecification);
    
    /// <inheritdoc />
    public object? Instance { get; set; }
    
    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => null;
    
    /// <inheritdoc />
    public object?[]? Inputs { get; init; } = null;

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata => null;
    
    /// <inheritdoc />
    public object?[]? Outputs { get; init; } = null;
    
    /// <inheritdoc />
    public Result? Result { get; private set; }
    
    /// <inheritdoc />
    public int ReturnIndex { get; set; }

    private static readonly ProcessPointMetadata[] ProcessPoint =
    [
        new(0, "True", "Triggered when the condition is true."),
        new(1, "False", "Triggered when the condition is false."),
    ];
    
    /// <inheritdoc />
    public ProcessPointMetadata[] ProcessPointMetadata => ProcessPoint;

    /// <inheritdoc />
    public void Execute()
    {
        bool result;
        
        try
        {
            result = ((ISpecification)Instance!).Result;
        }
        catch(Exception ex)
        {
            Result = new Result(false, false, ex, "Failed to calculate the result.");
            return;
        }

        ReturnIndex = result ? 0 : 1;
    }
}
