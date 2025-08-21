namespace Flow.Automation.Decisioning;

public class DelegateSpecification<T>(T obj, Predicate<T> predicate, params object[]? args) : Specification<T>(obj, args)
{
    public override bool Result => Predicate.Invoke(Obj);

    public Predicate<T> Predicate { get; set; } = predicate;
}