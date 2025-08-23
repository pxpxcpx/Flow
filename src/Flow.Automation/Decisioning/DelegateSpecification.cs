using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning;

public class DelegateSpecification<T>(Predicate<T> predicate, T obj, params object[]? args) 
    : Specification<T>(obj, args)
{
    public Predicate<T> Predicate { get; set; } = predicate;

    public override bool Result => Predicate.Invoke(Obj);
}