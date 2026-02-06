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

    /// <summary>
    /// Evaluate the object with the <see cref="condition"/>.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="obj">Object to be evaluated.</param>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public static bool Evaluate<TObject>(this IParamCondition<TObject> condition, TObject obj)
        => condition.Evaluate(obj);

    /// <summary>
    /// Create a <see cref="TCondition"/> using input parameters as conditions
    /// </summary>
    /// <param name="args"></param>
    /// <typeparam name="TCondition"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public static TCondition Create<TCondition, TObject>(params object?[]? args)
        where TCondition : IParamCondition<TObject>, new()
        => new() { Conditions = args };

    /// <summary>
    /// Combine two conditions into one using an operator.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operator"></param>
    /// <param name="right"></param>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public static CompositeCondition<TObject> Combine<TObject>(this ICondition left, LogicalOperator @operator,
        ICondition right)
        => new(left, @operator, right);

    public static ICondition True()
        => BooleanSpecification.True;

    public static ICondition False()
        => BooleanSpecification.False;
}