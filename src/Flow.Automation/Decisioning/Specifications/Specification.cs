using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Specifications;

/// <summary>
/// Base class of <see cref="IParamSpecification{T}"/>.
/// </summary>
/// <param name="obj"></param>
/// <param name="args"></param>
/// <typeparam name="T"></typeparam>
public abstract class Specification<T>(T? obj, params object?[]? args) : IParamSpecification<T>
{
    public T? Obj { get; set; } = obj;

    public object?[]? Args { get; set; } = args;

    public virtual bool Result => IsSatisfiedBy();
    
    protected void Assign(T? obj = default, params object?[]? args)
    {
        Obj = obj is null ? Obj : obj;
        Args = args is null || args.Length == 0 ? Args : args;
    }
    
    public abstract bool IsSatisfiedBy(T? obj = default, params object?[]? args);
}
