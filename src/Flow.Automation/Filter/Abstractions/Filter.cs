namespace Flow.Automation.Filter.Abstractions;

public abstract class Filter<TObj>(TObj obj, params object[]? args) : IFilter<TObj>
{
    public TObj Source { get; init; } = obj;

    public object[]? Args { get; init; } = args;
    
    public virtual TObj? Filtered => Filtering();

    public virtual TObj? Filtering() => default;
}