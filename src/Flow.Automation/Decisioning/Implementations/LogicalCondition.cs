using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Implementations;

/// <summary>
/// Used to calculate the results of two <see cref="ICondition"/>
/// </summary>
/// <param name="a">Left param</param>
/// <param name="b">Right param</param>
/// <param name="operator"><see cref="LogicalOperator"/></param>
public sealed class LogicalCondition(ICondition a, ICondition b, LogicalOperator @operator) : ICondition
{
    public bool Result => Evaluate(a, b, @operator);

    /// <summary>
    /// Evaluate two boolean values with <see cref="LogicalOperator"/>.
    /// </summary>
    /// <param name="a">Value a</param>
    /// <param name="b">Value b</param>
    /// <param name="op">Operator between a and b.</param>
    /// <returns>Result of evaluation, boolean.</returns>
    /// <exception cref="InvalidOperationException">
    /// Throws when operator is <see cref="LogicalOperator.None"/> or <see cref="LogicalOperator.Not"/>.
    /// </exception>
    public static bool Evaluate(bool a, bool b, LogicalOperator op)
    {
        // Reverse if op contains a Not Operator
        // e.g., Nor (0110) -> Not (0100) + Or (0010), and the part of Or op will be reversed. 
        if (op.HasFlag(LogicalOperator.Not))
        {
            var innerOp = op & ~LogicalOperator.Not;
            return !Evaluate(a, b, innerOp);
        }
        
        return op switch
        {
            LogicalOperator.And => a && b,
            LogicalOperator.Or => a || b,
            LogicalOperator.Xor => a ^ b,
            LogicalOperator.None => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };
    }
    
    /// <summary>
    /// Evaluate two <see cref="ICondition"/> value with <see cref="LogicalOperator"/>.
    /// </summary>
    /// <param name="a">Value a</param>
    /// <param name="b">Value b</param>
    /// <param name="op">Operator between a and b.</param>
    /// <returns>Result of evaluation, boolean.</returns>
    public static bool Evaluate(ICondition a, ICondition b, LogicalOperator op)
        => Evaluate(a.Result, b.Result, op);
}