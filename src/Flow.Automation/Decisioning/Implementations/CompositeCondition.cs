using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Implementations;

/// <summary>
/// Combine two <see cref="ICondition"/>.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public class CompositeCondition<TObject> : Condition<TObject>
{
    public ICondition Left { get; set; }
    public LogicalOperator Operator { get; set; }
    public ICondition Right { get; set; }

    public CompositeCondition(ICondition left, LogicalOperator @operator, ICondition right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    /// <summary>
    /// Evaluate all specifications for <see cref="obj"/> with operator.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Evaluate(TObject? obj = default)
    {
        bool l;
        bool r;
        
        if (Left is IParamCondition<TObject> ls)
        {
            l = ls.Evaluate(obj);
        }
        else
        {
            l = Left.Result;
        }

        if(Right is IParamCondition<TObject> rs)
        {
            r = rs.Evaluate(obj);
        }
        else
        {
            r = Right.Result;
        }
        
        return LogicalCondition.Evaluate(l, r, Operator);
    }
}