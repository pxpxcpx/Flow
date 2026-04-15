using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.BuiltIn.ControlStatements;

/// <summary>
/// “If” conditional statement node.
/// </summary>
public class IfStatement : IExecutableNode, IControlStatement
{
    #region Metadata

    private static readonly NodeMetadata NodeMetadata = new()
    {
        Id = new Guid("7AE6EB1F-5B2C-4495-99D1-855CCF8B6FD0"),
        Name = "If statement",
        Description = "Used as a conditional statement node."
    };
    
    /// <inheritdoc />
    public NodeMetadata Metadata => NodeMetadata;

    #endregion

    #region Runtime Info

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc />
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }
    
    /// <inheritdoc />
    public Result? Result { get; private set; }

    #endregion
    
    #region IO

    private static readonly ParameterMetadata[]? InputMetadata =
    [
        new ParameterMetadata
        {
            Index = 0,
            Name = "Bool Value",
            Description = "True or False",
            Type = typeof(bool),
            IsRequired = true,
            DefaultValue = false
        }
    ];

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => InputMetadata;
    
    /// <inheritdoc />
    public object?[]? Inputs { get; init; } = new object?[1];

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata => null;
    
    /// <inheritdoc />
    public object?[]? Outputs { get; init; } = null;

    #endregion

    private IfStatement(IfStatement old)
    {

    }

    #region Process Point Controlling

    /// <inheritdoc />
    public int ReturnedPort { get; set; }

    private static readonly ProcessPointMetadata[] ProcessPoint =
    [
        new(0, "True", "Triggered when the condition is true."),
        new(1, "False", "Triggered when the condition is false."),
    ];
    
    /// <inheritdoc />
    public ProcessPointMetadata[] ProcessPointMetadata => ProcessPoint;

    #endregion

    /// <inheritdoc />
    public void Execute()
    {
        bool boolValue;
        
        try
        {
            boolValue = (bool)Inputs![0]!;
        }
        catch(Exception ex)
        {
            Result = new Result(false, false, ex, "Failed to calculate the result.");
            return;
        }

        ReturnedPort = boolValue ? 0 : 1;
    }

    public INode? Clone()
    {
        throw new NotImplementedException();
    }
}
