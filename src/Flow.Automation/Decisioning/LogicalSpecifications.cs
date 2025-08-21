using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning;

/// <summary>
/// Used to calculate the results of two <see cref="ISpecification"/>
/// </summary>
/// <param name="a">Left param</param>
/// <param name="b">Right param</param>
/// <param name="operator"><see cref="LogicalOperator"/></param>
public sealed class LogicalOperation(ISpecification a, ISpecification b, LogicalOperator @operator) : ISpecification
{
    public bool Result => Evaluate(a, b, @operator);

    public static bool Evaluate(ISpecification a, ISpecification b, LogicalOperator op)
    {
        // Reverse if op contains a Not Operator
        // e.g. Nor (0110) -> Not (0100) + Or (0010), and the part of Or operator will be reversed. 
        if (op.HasFlag(LogicalOperator.Not))
        {
            var innerOp = op & ~LogicalOperator.Not;
            return !Evaluate(a, b, innerOp);
        }
        
        return op switch
        {
            LogicalOperator.And => a.Result && b.Result,
            LogicalOperator.Or => a.Result || b.Result,
            LogicalOperator.Xor => a.Result && b.Result,
            LogicalOperator.None => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };
    }
}