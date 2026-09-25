using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;
using Flow.Shared.Utils;

namespace Flow.BuiltIn.ControlStatements;

/// <summary>
/// “If” conditional statement node.
/// </summary>
public class IfStatement : IExecutableNode, IControlStatement
{
    private static readonly NodeMetadata NodeMetadata = new()
    {
        Id = new Guid("7AE6EB1F-5B2C-4495-99D1-855CCF8B6FD0"),
        Identifier = "If statement",
        Description = "Used as a conditional statement node."
    };

    /// <inheritdoc />
    public NodeMetadata Metadata => NodeMetadata;

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc />
    public bool IsEnabled { get; set; }
    
    /// <inheritdoc />
    public VoidResult? Result { get; private set; }
    
    /// <inheritdoc />
    public NodeStates State { get; private set; }
    
    /// <inheritdoc />
    public NodeStates PreviousState { get; private set; }

    private static readonly ParameterMetadata[]? InputMetadata =
    [
        new ParameterMetadata
        {
            Index = 0,
            Identifier = "Bool Value",
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

    public IfStatement()
    {
    }
    
    private IfStatement(IfStatement old)
    {
    }

    /// <inheritdoc />
    public int ReturnedPort { get; set; }

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
        bool boolValue;
        
        try
        {
            boolValue = (bool)Inputs![0]!;
        }
        catch(Exception ex)
        {
            Result = VoidResult.Err(ex);
            return;
        }

        ReturnedPort = boolValue ? 0 : 1;
    }

    // TODO
    public INode Clone()
    {
        throw new NotImplementedException();
    }
    
    // TODO: (Possible) special logical for if statement node state transition.
    public bool Fire(NodeEvents @event)
    {
        PreviousState = State;
        State = NodeExtensions.DefaultStateTransform(PreviousState, @event);
        return true;
    }
}
