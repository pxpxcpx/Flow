using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;
using Flow.Shared.Utils;

namespace Flow.Tests.FeasibilityTest.StaticNodeV2;

public partial class StaticNodeExampleV2
{
    public class AddNode : INode, IExecutable
    {
        private static readonly NodeMetadata NodeMetadata = new()
        {
            Id = new Guid(g: "D838E6F8-A8E4-4FDE-A91F-E56747E98B82"), // Replaceable, generated once for the same method.
            Identifier = "Add",
            Description = "Return the sum of two numbers."
        };
        
        public NodeMetadata Metadata => NodeMetadata;
        
        public Guid RuntimeId { get; init; }
        
        public bool IsEnabled { get; }
        
        /// <inheritdoc />
        public NodeStates State { get; private set; }
    
        /// <inheritdoc />
        public NodeStates PreviousState { get; private set; }
        
        private static readonly ParameterMetadata[]? InputMetadata =
        [
            new ParameterMetadata
            {
                Index = 0,
                Identifier = "Number A", 
                Description = "First number to add.",
                Type = typeof(int), 
                IsRequired = true, 
                DefaultValue = 0
            },
            new ParameterMetadata
            {
                Index = 1,
                Identifier = "Number B",
                Description = "Second Number to add.",
                Type = typeof(int), 
                IsRequired = true,
                DefaultValue = 0
            }
        ];
        public ParameterMetadata[]? InputVariableMetadata => InputMetadata;
        public object?[]? Inputs { get; init; }
        
        private static readonly ParameterMetadata[]? OutputMetadata =
        [
            new ParameterMetadata
            {
                Index = 0,
                Identifier = "Sum",
                Description = "Sum of two numbers.",
                Type = typeof(int),
                IsRequired = true,
            }
        ];
        public ParameterMetadata[]? OutputVariableMetadata => OutputMetadata;
        public object?[]? Outputs { get; init; }
        
        public VoidResult? Result { get; private set; }
        
        public void Execute()
        {
            try
            {
                Add(a: (int)Inputs![0]!, b: (int)Inputs![1]!);
            }
            catch (Exception ex)
            {
                Result = VoidResult.Err(ex);
            }
        }

        public INode Clone()
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc />
        public bool Fire(NodeEvents @event)
        {
            PreviousState = State;
            State = NodeExtensions.DefaultStateTransform(PreviousState, @event);
            return true;
        }
    }
}