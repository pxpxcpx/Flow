using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Decisioning.Specifications;

namespace Flow.Automation.Decisioning;

public static class SpecExtensions
{
    /// <summary>
    /// Convert a <see cref="Predicate{T}"/> to a <see cref="DelegateSpecification{T}"/>.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DelegateSpecification<T> ToSpecification<T>(this Predicate<T> predicate, T? obj = default, params object?[]? args) 
        => new(predicate, obj, args);
    
    /// <summary>
    /// Convert a <see cref="Boolean"/> value to a <see cref="BooleanSpecification"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ISpecification ToSpecification(this bool value)
        => value ? BooleanSpecification.True : BooleanSpecification.False;
    
    /// <summary>
    /// Convert an <see cref="ISpecification"/> instance to a <see cref="Predicate{T}"/>/.
    /// </summary>
    /// <param name="specification"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Predicate<T> ToPredicate<T>(this IParamSpecification<T> specification)
        => obj => specification.IsSatisfiedBy(obj);
    
    /// <inheritdoc cref="LogicalSpecification.Evaluate(ISpecification, ISpecification, LogicalOperator)"/>
    public static bool Evaluate(this ISpecification left, ISpecification right, LogicalOperator @operator)
        => LogicalSpecification.Evaluate(left, right, @operator);
    
    /// <inheritdoc cref="LogicalSpecification.Evaluate(ISpecification, ISpecification, LogicalOperator)"/>
    public static bool Evaluate(this ISpecification[] specifications, LogicalOperator op)
    {
        if (specifications.Length < 1)
            throw new ArgumentException("At least one specification is required", nameof(specifications));
        
        var result = specifications[0].Result;
        for (var i = 1; i < specifications.Length; i++)
            result = LogicalSpecification.Evaluate(result, specifications[i].Result, op);
        
        return result;
    }
}