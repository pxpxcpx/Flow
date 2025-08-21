
using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning;

public abstract class Specification<T>(T obj, params object[]? args) : ISpecification
{
    public T Obj { get; init; } = obj;

    public object[]? Args { get; set; } = args;

    public virtual bool Result => IsSatisfiedBy();

    public virtual bool IsSatisfiedBy() => false;
}