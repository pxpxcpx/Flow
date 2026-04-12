namespace Flow.Shared.Abstractions;

public interface IFactory<out T>
{
    /// <summary>
    /// Create instance of <see cref="T"/>.
    /// </summary>
    /// <returns></returns>
    T? Create();
}

public interface IFactory<out T, in TParam>
{
    /// <summary>
    /// Create instance of <see cref="T"/> with <see cref="TParam"/>.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    T? Create(TParam param);
}