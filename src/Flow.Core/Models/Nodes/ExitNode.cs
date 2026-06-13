using System.Diagnostics.CodeAnalysis;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Core.Models.Nodes;

internal sealed class ExitNode : IExecutableNode
{
    [NotNull] internal Function? Function;

    /// <inheritdoc />
    public NodeMetadata Metadata => Meta;

    private static readonly NodeMetadata Meta = new NodeMetadata()
    {
        Name = "Entry Node",
        Description = "As an entry point of a function, usually there can only be one within each function.",
        Id = Guid.Parse("ACCE82DB-F41E-4F4C-8F02-4302A047DA3E"),
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
    public object?[] Inputs => _outputs.ToArray();
    
    /// <inheritdoc />
    public object?[] Outputs => _outputs.ToArray();

    private readonly List<object?> _outputs;
    
    /// <inheritdoc />
    public VoidResult? Result { get; private set; }

    internal ExitNode(Function function)
    {
        RuntimeId = Guid.NewGuid();
        
        Function = function;
        _outputs = [];
    }

    private ExitNode(ExitNode old)
    {
        RuntimeId = Guid.NewGuid();

        Function = old.Function;
        _outputs = old._outputs;
    }

    /// <inheritdoc />
    public void Execute()
    {
        if (Function.Outputs == null || Function.Outputs.Length == 0)
        {
            Result = VoidResult.Ok();
            return;
        }

        foreach (var output in Function.Outputs)
            _outputs.Add(output);
        
        Result = VoidResult.Ok();
    }

    public INode Clone()
        => new ExitNode(this);
}