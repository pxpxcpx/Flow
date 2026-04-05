using System.Diagnostics.CodeAnalysis;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Core.Models.Nodes;

internal sealed class ExitNode : IExecutableNode
{
    [NotNull] private readonly Function? _function;

    /// <inheritdoc />
    public NodeMetadata Metadata => Meta;

    private static readonly NodeMetadata Meta = new NodeMetadata()
    {
        Name = "Entrance Node",
        Description = "As an entry point of a function, usually there can only be one within each function.",
        Id = Guid.Parse("ACCE82DB-F41E-4F4C-8F02-4302A047DA3E"),
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
    public object?[] Inputs => _outputs.ToArray();
    
    /// <inheritdoc />
    public object?[] Outputs => _outputs.ToArray();

    private readonly List<object?> _outputs;
    
    /// <inheritdoc />
    public Result? Result { get; private set; }

    internal ExitNode(Function function)
    {
        RuntimeId = Guid.NewGuid();
        
        _function = function;
        _outputs = [];
    }

    /// <inheritdoc />
    public void Execute()
    {
        if (_function.Outputs == null || _function.Outputs.Length == 0)
        {
            Result = Result.Completed;
            return;
        }

        foreach (var output in _function.Outputs)
            _outputs.Add(output);
        
        Result = Result.Completed;
    }
}