namespace Flow.Automation.Filter.Abstractions;

/// <summary>
/// Definite the interface of a filter, 
/// only caring about the <see cref="Filtered"/> result
/// </summary>
/// <typeparam name="TSource"></typeparam>
public interface IFilter<out TSource>
{
    /// <summary>
    /// The result after filtering
    /// </summary>
    TSource? Filtered { get; }
}