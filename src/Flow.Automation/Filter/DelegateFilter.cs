using Flow.Automation.Filter.Abstractions;

namespace Flow.Automation.Filter;

public class DelegateFilter<TSource>(Func<TSource, object[]?, TSource?> func, TSource source, params object[]? args) 
    : Filter<TSource>(source, args)
{
    public Func<TSource, object[]?, TSource?> FilterFunc { get; set; } = func;
    
    public override TSource? Filtered => FilterFunc(Source, Args);
}