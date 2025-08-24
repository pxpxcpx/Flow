namespace Flow.Automation.Decisioning.Specifications;

/// <summary>
/// Accept a <see cref="Predicate">delegate</see> as a condition of judgment.
/// </summary>
/// <param name="predicate">Delegate</param>
/// <param name="obj">Nullable, object to be evaluated.</param>
/// <param name="args">Nullable, parameters used to pass in the delegate.</param>
/// <typeparam name="T">Type of the <see cref="Specification{T}.Obj"/></typeparam>
public class DelegateSpecification<T>(Predicate<T> predicate, T? obj, params object?[]? args) 
    : Specification<T>(obj, args)
{
    public Predicate<T> Predicate { get; set; } = predicate;

    public override bool Result => IsSatisfiedBy();

    public override bool IsSatisfiedBy(T? obj = default, params object?[]? args)
    {
        Assign(obj, args);
        return Predicate.Invoke(Obj ?? throw new ArgumentNullException(nameof(obj)));
    }

    public static implicit operator DelegateSpecification<T>(Predicate<T> predicate)
        => predicate.ToSpecification();
}