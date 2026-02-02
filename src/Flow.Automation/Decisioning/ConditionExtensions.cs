using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Decisioning.Implementations;

namespace Flow.Automation.Decisioning;

public static class ConditionExtensions
{
    /// <summary>
    /// Convert a <see cref="Predicate{T}"/> to a <see cref="DelegateCondition{T}"/>.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DelegateCondition<T> ToCondition<T>(this Predicate<T> predicate, params object?[]? args) 
        => new(predicate, args);
    
    /// <summary>
    /// Convert a <see cref="Boolean"/> value to a <see cref="BooleanSpecification"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ICondition ToCondition(this bool value)
        => value ? BooleanSpecification.True : BooleanSpecification.False;
    
    /// <summary>
    /// Convert an <see cref="ICondition"/> instance to a <see cref="Predicate{T}"/>/.
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Predicate<T> ToPredicate<T>(this IParamCondition<T> condition)
        => obj => condition.Evaluate(obj);
    
    /// <inheritdoc cref="LogicalCondition.Evaluate(ICondition, ICondition, LogicalOperator)"/>
    public static bool Evaluate(this ICondition left, ICondition right, LogicalOperator @operator)
        => LogicalCondition.Evaluate(left, right, @operator);
    
    /// <inheritdoc cref="LogicalCondition.Evaluate(ICondition, ICondition, LogicalOperator)"/>
    public static bool Evaluate(this ICondition[] specifications, LogicalOperator op)
    {
        if (specifications.Length < 1)
            throw new ArgumentException("At least one condition is required", nameof(specifications));
        
        var result = specifications[0].Result;
        for (var i = 1; i < specifications.Length; i++)
            result = LogicalCondition.Evaluate(result, specifications[i].Result, op);
        
        return result;
    }
    
    public static bool Evaluate<TObject>(this IParamCondition<TObject> condition, TObject obj)
        => condition.Evaluate(obj);
    
    public static ICondition True()
        => BooleanSpecification.True;
    
    public static ICondition False()
        => BooleanSpecification.False;
    
    public static TSpecification Create<TSpecification, TObject>(params object?[]? args)
        where TSpecification : IParamCondition<TObject>, new() 
        => new(){ Conditions = args };

    public static CompositeCondition<TObject> Combine<TObject>(this ICondition left, LogicalOperator @operator, ICondition right)
        => new(left, @operator, right);
}