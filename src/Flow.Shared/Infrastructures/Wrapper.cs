using Flow.Shared.Abstractions;
using Flow.Shared.Results;

namespace Flow.Shared.Infrastructures;

//
// If you have reached this point,
// then perhaps you have also realized that this class or this "wrapper pattern"
// does not seem like a long-term solution to the problem of generic types.
// ¯\_(ツ)_/¯
//

/// <summary>
/// A non-generic container template for a generic object.
/// Property <see cref="Type"/> will internally always stay related to the <see cref="Object"/>.
/// Can implement the specific methods of the corresponding type internally.
/// </summary>
public abstract class Wrapper : IWrapper
{
    private readonly Type _type;

    private object _object;

    public Type Type => _type;

    public virtual object Object
    {
        get => _object;
        set => SetObject(value);
    }

    public Wrapper(object obj)
    {
        _type = obj.GetType();
        _object = obj;
    }

    public Wrapper(Type type, object obj)
    {
        if (!obj.GetType().IsAssignableFrom(type))
            throw new InvalidCastException();

        _type = type;
        _object = obj;
    }

    /// <summary>
    /// Try to get the object with specific type.
    /// If <see cref="T"/> b doesn't inherit from <see cref="Type"/>, a <see cref="InvalidCastException"/> will be thrown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Result<T, InvalidCastException> GetObject<T>()
        where T : notnull
    {
        if (!typeof(T).IsAssignableFrom(_type))
            return Result<T, InvalidCastException>.Err(new InvalidCastException());

        try
        {
            return Result<T, InvalidCastException>.Ok((T)_object);
        }
        catch (InvalidCastException ice)
        {
            return Result<T, InvalidCastException>.Err(ice);
        }
    }

    /// <summary>
    /// Process object with custom functions.
    /// </summary>
    /// <param name="action"></param>
    public void GetObject(Action<Type, object> action)
        => action(_type, _object);

    /// <summary>
    /// Set the <see cref="Object"/> if <see cref="T"/> inherits from <see cref="Type"/>.
    /// </summary>
    /// <param name="newObj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool SetObject<T>(T newObj)
        where T : notnull
    {
        if (!typeof(T).IsAssignableFrom(_type))
            return false;

        _object = newObj;
        return true;
    }

    /// <summary>
    /// Set the <see cref="Object"/> if its type inherits from <see cref="Type"/>
    /// </summary>
    /// <param name="newObj"></param>
    /// <returns></returns>
    public bool SetObject(object newObj)
    {
        if (!newObj.GetType().IsAssignableFrom(_type))
            return false;

        _object = newObj;
        return true;
    }
}