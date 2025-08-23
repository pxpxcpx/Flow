
namespace Flow.Automation.Decisioning.Abstractions;

public abstract class Specification<T>(T obj, params object[]? args) : ISpecification
{
    public T Obj { get; init; } = obj;

    public object[]? Args { get; init; } = args;

    public virtual bool Result => IsSatisfiedBy();

    public virtual bool IsSatisfiedBy() => false;
}