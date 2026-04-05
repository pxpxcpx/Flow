using System.Diagnostics.CodeAnalysis;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Core.Models.Nodes;

internal sealed class EntranceNode : IExecutableNode
{
    [NotNull] private readonly Function? _function;

    /// <inheritdoc />
    public NodeMetadata Metadata => Meta;

    private static readonly NodeMetadata Meta = new NodeMetadata()
    {
        Name = "Entrance Node",
        Description = "As an entry point of a function, usually there can only be one within each function.",
        Id = Guid.Parse("B522839C-3BA1-4CDD-B128-6BDFE09A1022"),
    };

    /// <inheritdoc />
    public Guid RuntimeId { get; init; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata => _function.InputVariableMetadata;

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata =>  _function.InputVariableMetadata;

    /// <inheritdoc />
    public object?[] Inputs => _inputs.ToArray();
    
    private readonly List<object?> _inputs;

    /// <inheritdoc />
    public object?[] Outputs => _inputs.ToArray();

    /// <inheritdoc />
    public Result? Result { get; private set; }

    internal EntranceNode(Function function)
    {
        RuntimeId = Guid.NewGuid();
        
        _function = function;
        _inputs = [];
    }

    /// <inheritdoc />
    public void Execute()
    {
        if (_function.Inputs == null || _function.Inputs.Length == 0)
        {
            Result = Result.Completed;
            return;
        }

        foreach (var input in _function.Inputs)
            _inputs.Add(input);
        
        Result = Result.Completed;
    }
}