using System.Diagnostics.CodeAnalysis;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;
using Flow.Shared.Utils;

namespace Flow.Core.Models.Nodes;

internal sealed class EntryNode : IExecutableNode
{
    [NotNull] internal Function? Function;
    
    public NodeStates State { get; private set; }
    
    public NodeStates PreviousState { get; private set; }

    /// <inheritdoc />
    public NodeMetadata Metadata => Meta;

    private static readonly NodeMetadata Meta = new NodeMetadata()
    {
        Identifier = "Entry Node",
        Description = "As an entry point of a function, usually there can only be one within each function.",
        Id = Guid.Parse("B522839C-3BA1-4CDD-B128-6BDFE09A1022"),
    };

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }
    
    /// <inheritdoc/>
    public bool IsEnabled => true;

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => Function.InputVariableMetadata;

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata =>  Function.InputVariableMetadata;

    /// <inheritdoc />
    public object?[] Inputs => [.. _inputs];
    
    private readonly List<object?> _inputs;

    /// <inheritdoc />
    public object?[] Outputs => [.. _inputs];

    /// <inheritdoc />
    public VoidResult? Result { get; private set; }

    internal EntryNode(Function function)
    {
        RuntimeId = Guid.NewGuid();
        
        Function = function;
        _inputs = [];
    }

    private EntryNode(EntryNode old)
    {
        RuntimeId = Guid.NewGuid();

        Function = old.Function;
        _inputs = old._inputs;
    }
    
    public bool Fire(NodeEvents @event)
    {
        PreviousState = State;
        State = NodeExtensions.DefaultStateTransform(PreviousState, @event);
        return true;
    }

    /// <inheritdoc />
    public void Execute()
    {
        if (Function.Inputs == null || Function.Inputs.Length == 0)
        {
            Result = VoidResult.Ok();
            return;
        }

        foreach (var input in Function.Inputs)
            _inputs.Add(input);
        
        Result = VoidResult.Ok();
    }

    public INode Clone()
        => new EntryNode(this);
}