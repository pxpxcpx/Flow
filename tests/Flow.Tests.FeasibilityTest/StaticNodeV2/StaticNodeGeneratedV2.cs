using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Tests.FeasibilityTest.StaticNodeV2;

public partial class StaticNodeExampleV2
{
    public class AddNode : INode, IExecutable
    {
        private static readonly NodeMetadata NodeMetadata = new()
        {
            Id = new Guid(g: "D838E6F8-A8E4-4FDE-A91F-E56747E98B82"), // Replaceable, generated once for the same method.
            Name = "Add",
            Description = "Return the sum of two numbers."
        };
        
        public NodeMetadata Metadata => NodeMetadata;
        
        public Guid RuntimeId { get; init; }
        
        public NodeStatus Status { get; set; }
        
        private static readonly ParameterMetadata[]? InputMetadata =
        [
            new ParameterMetadata(
                Index: 0, 
                Name: "Number A", 
                Description: "First number to add.", 
                Type: typeof(int), 
                IsRequired: true, 
                DefaultValue: 0),
            new ParameterMetadata(
                Index: 1, 
                Name: "Number B", 
                Description: "Second Number to add.", 
                Type: typeof(int), 
                IsRequired: true, 
                DefaultValue: 0)
        ];
        public ParameterMetadata[]? InputVariableMetadata => InputMetadata;
        public object?[]? Inputs { get; init; }
        
        private static readonly ParameterMetadata[]? OutputMetadata =
        [
            new ParameterMetadata(
                Index: 0, 
                Name: "Sum", 
                Description: "Sum of two numbers.", 
                Type: typeof(int), 
                IsRequired: true, 
                DefaultValue: 0)
        ];
        public ParameterMetadata[]? OutputVariableMetadata => OutputMetadata;
        public object?[]? Outputs { get; init; }
        
        public Result? Result { get; private set; }
        
        public void Execute()
        {
            try
            {
                Add(a: (int)Inputs![0]!, b: (int)Inputs![1]!);
            }
            catch (Exception ex)
            {
                Result = new Result(
                    IsCompleted: false, 
                    IsSuccess: false, 
                    Exception: ex, 
                    Message: "Failed to calculate the result.");
            }
        }
    }
}