namespace Flow.Automation.Decisioning.Abstractions;

/// <summary>
/// Inherits from <see cref="ISpecification"/>,
/// add the instance as the data to be evaluated.
/// </summary>
/// <typeparam name="T">Type of the <see cref="Obj"/>.</typeparam>
public interface IParamSpecification<T> : ISpecification
{
    /// <summary>
    /// Object to be evaluated.
    /// </summary>
    public T? Obj { get; set; }
    
    /// <summary>
    /// Parameters to be passed to the <see cref="IsSatisfiedBy"/> method.
    /// </summary>
    public object?[]? Args { get; set; }

    /// <summary>
    /// A method for determining whether the data meets the conditions.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool IsSatisfiedBy(T? obj = default, params object?[]? args);
}