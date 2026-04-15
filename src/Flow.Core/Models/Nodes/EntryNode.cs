using System.Diagnostics.CodeAnalysis;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Core.Models.Nodes;

internal sealed class EntryNode : IExecutableNode
{
    [NotNull] internal Function? Function;

    /// <inheritdoc />
    public NodeMetadata Metadata => Meta;

    private static readonly NodeMetadata Meta = new NodeMetadata()
    {
        Name = "Entry Node",
        Description = "As an entry point of a function, usually there can only be one within each function.",
        Id = Guid.Parse("B522839C-3BA1-4CDD-B128-6BDFE09A1022"),
    };

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }
    
    /// <inheritdoc/>
    public bool IsEnabled => true;

    /// <inheritdoc />
    public NodeStatus Status { get; set; }

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => Function.InputVariableMetadata;

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata =>  Function.InputVariableMetadata;

    /// <inheritdoc />
    public object?[] Inputs => _inputs.ToArray();
    
    private readonly List<object?> _inputs;

    /// <inheritdoc />
    public object?[] Outputs => _inputs.ToArray();

    /// <inheritdoc />
    public Result? Result { get; private set; }

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

    /// <inheritdoc />
    public void Execute()
    {
        if (Function.Inputs == null || Function.Inputs.Length == 0)
        {
            Result = Result.Completed;
            return;
        }

        foreach (var input in Function.Inputs)
            _inputs.Add(input);
        
        Result = Result.Completed;
    }

    public INode? Clone()
        => new EntryNode(this);
}