using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Decisioning.Specifications;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.BuiltIn.ControlStatements.Specifications;

public class LogicalSpecificationNode : INode
{
    #region Metadata

    private static readonly NodeMetadata NodeMetadata = new()
    {
        Id = new Guid("6D59D8BB-5407-4B96-B901-D4437046A789"),
        Name = "Logical operations",
        Description = "Used for logical calculations between left and right values."
    };
    
    /// <inheritdoc />
    public NodeMetadata Metadata => NodeMetadata;

    #endregion

    #region Runtime Info

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }
    
    /// <inheritdoc />
    public NodeStatus Status { get; set; }
    
    /// <inheritdoc />
    public Result? Result { get; private set; }

    #endregion
    
    #region IO

    private static readonly ParameterMetadata[]? InputMetadata =
    [
        new ParameterMetadata(0, "Value 1", "", typeof(object), true),
        new ParameterMetadata(1, "Value 2", "", typeof(object), true),
        new ParameterMetadata(2, "Operator", "Calculation symbol used to compute the left and right values", typeof(LogicalOperator), true, LogicalOperator.And)
    ];

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => InputMetadata;
    
    /// <inheritdoc />
    public object?[]? Inputs { get; init; } = new object?[2];
    
    private static readonly ParameterMetadata[]? OutputMetadata =
    [
        new ParameterMetadata(0, "Result", "Result of the operation.", typeof(bool), false)
    ];

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata => OutputMetadata;
    
    /// <inheritdoc />
    public object?[]? Outputs { get; init; } = new object?[1];

    #endregion
    
    /// <inheritdoc />
    public void Execute()
    {
        var value1 = false;
        var value2 = false;
        var @operator = LogicalOperator.And;
        try
        {
            value1 = (bool)Inputs![0]!;
            value2 = (bool)Inputs![1]!;
            @operator = (LogicalOperator)Inputs![2]!;
        }
        catch(Exception ex)
        {
            Result = new Result(false, false, ex, "Failed to calculate the result.");
        }
        
        bool result;
        try
        {
            result = LogicalSpecification.Evaluate(value1, value2, @operator);
        }
        catch(Exception ex)
        {
            Result = new Result(false, false, ex, ex.Message);
            return;
        }
        Outputs![0] = result;
    }
}