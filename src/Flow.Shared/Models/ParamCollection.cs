using System.Runtime.CompilerServices;
using Flow.Shared.Abstractions;

namespace Flow.Shared.Models;

//
// Provides param collections from 1 parameter to 8 parameters.
// Use interface IParamCollection if you don't know the num of params. 
//

/// <inheritdoc />
public struct ParamCollection<T1>(
    T1 param1)
    : IParamCollection
{
    public int Count => 1;

    private T1 _param1 = param1;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }


    /// <inheritdoc />/// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2>(
    T1 param1,
    T2 param2)
    : IParamCollection
{
    public int Count => 2;

    private T1 _param1 = param1;
    private T2 _param2 = param2;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3>(
    T1 param1,
    T2 param2,
    T3 param3)
    : IParamCollection
{
    public int Count => 3;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3, T4>(
    T1 param1,
    T2 param2,
    T3 param3,
    T4 param4)
    : IParamCollection
{
    public int Count => 4;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;
    private T4 _param4 = param4;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    public T4 Param4
    {
        get => _param4;
        set => _param4 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            3 => typeof(T) != typeof(T4) ? throw new InvalidCastException() : Unsafe.As<T4, T>(ref _param4),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            case 3:
                if (typeof(T) != typeof(T4)) throw new InvalidCastException();
                Unsafe.As<T4, T>(ref _param4) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3, T4, T5>(
    T1 param1,
    T2 param2,
    T3 param3,
    T4 param4,
    T5 param5)
    : IParamCollection
{
    public int Count => 5;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;
    private T4 _param4 = param4;
    private T5 _param5 = param5;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    public T4 Param4
    {
        get => _param4;
        set => _param4 = value;
    }

    public T5 Param5
    {
        get => _param5;
        set => _param5 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            3 => typeof(T) != typeof(T4) ? throw new InvalidCastException() : Unsafe.As<T4, T>(ref _param4),
            4 => typeof(T) != typeof(T5) ? throw new InvalidCastException() : Unsafe.As<T5, T>(ref _param5),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            case 3:
                if (typeof(T) != typeof(T4)) throw new InvalidCastException();
                Unsafe.As<T4, T>(ref _param4) = value;
                return;
            case 4:
                if (typeof(T) != typeof(T5)) throw new InvalidCastException();
                Unsafe.As<T5, T>(ref _param5) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3, T4, T5, T6>(
    T1 param1,
    T2 param2,
    T3 param3,
    T4 param4,
    T5 param5,
    T6 param6)
    : IParamCollection
{
    public int Count => 6;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;
    private T4 _param4 = param4;
    private T5 _param5 = param5;
    private T6 _param6 = param6;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    public T4 Param4
    {
        get => _param4;
        set => _param4 = value;
    }

    public T5 Param5
    {
        get => _param5;
        set => _param5 = value;
    }

    public T6 Param6
    {
        get => _param6;
        set => _param6 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            3 => typeof(T) != typeof(T4) ? throw new InvalidCastException() : Unsafe.As<T4, T>(ref _param4),
            4 => typeof(T) != typeof(T5) ? throw new InvalidCastException() : Unsafe.As<T5, T>(ref _param5),
            5 => typeof(T) != typeof(T6) ? throw new InvalidCastException() : Unsafe.As<T6, T>(ref _param6),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            case 3:
                if (typeof(T) != typeof(T4)) throw new InvalidCastException();
                Unsafe.As<T4, T>(ref _param4) = value;
                return;
            case 4:
                if (typeof(T) != typeof(T5)) throw new InvalidCastException();
                Unsafe.As<T5, T>(ref _param5) = value;
                return;
            case 5:
                if (typeof(T) != typeof(T6)) throw new InvalidCastException();
                Unsafe.As<T6, T>(ref _param6) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3, T4, T5, T6, T7>(
    T1 param1,
    T2 param2,
    T3 param3,
    T4 param4,
    T5 param5,
    T6 param6,
    T7 param7)
    : IParamCollection
{
    public int Count => 7;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;
    private T4 _param4 = param4;
    private T5 _param5 = param5;
    private T6 _param6 = param6;
    private T7 _param7 = param7;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    public T4 Param4
    {
        get => _param4;
        set => _param4 = value;
    }

    public T5 Param5
    {
        get => _param5;
        set => _param5 = value;
    }

    public T6 Param6
    {
        get => _param6;
        set => _param6 = value;
    }

    public T7 Param7
    {
        get => _param7;
        set => _param7 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            3 => typeof(T) != typeof(T4) ? throw new InvalidCastException() : Unsafe.As<T4, T>(ref _param4),
            4 => typeof(T) != typeof(T5) ? throw new InvalidCastException() : Unsafe.As<T5, T>(ref _param5),
            5 => typeof(T) != typeof(T6) ? throw new InvalidCastException() : Unsafe.As<T6, T>(ref _param6),
            6 => typeof(T) != typeof(T7) ? throw new InvalidCastException() : Unsafe.As<T7, T>(ref _param7),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            case 3:
                if (typeof(T) != typeof(T4)) throw new InvalidCastException();
                Unsafe.As<T4, T>(ref _param4) = value;
                return;
            case 4:
                if (typeof(T) != typeof(T5)) throw new InvalidCastException();
                Unsafe.As<T5, T>(ref _param5) = value;
                return;
            case 5:
                if (typeof(T) != typeof(T6)) throw new InvalidCastException();
                Unsafe.As<T6, T>(ref _param6) = value;
                return;
            case 6:
                if (typeof(T) != typeof(T7)) throw new InvalidCastException();
                Unsafe.As<T7, T>(ref _param7) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

/// <inheritdoc />
public struct ParamCollection<T1, T2, T3, T4, T5, T6, T7, T8>(
    T1 param1,
    T2 param2,
    T3 param3,
    T4 param4,
    T5 param5,
    T6 param6,
    T7 param7,
    T8 param8)
    : IParamCollection
{
    public int Count => 8;

    private T1 _param1 = param1;
    private T2 _param2 = param2;
    private T3 _param3 = param3;
    private T4 _param4 = param4;
    private T5 _param5 = param5;
    private T6 _param6 = param6;
    private T7 _param7 = param7;
    private T8 _param8 = param8;

    public T1 Param1
    {
        get => _param1;
        set => _param1 = value;
    }

    public T2 Param2
    {
        get => _param2;
        set => _param2 = value;
    }

    public T3 Param3
    {
        get => _param3;
        set => _param3 = value;
    }

    public T4 Param4
    {
        get => _param4;
        set => _param4 = value;
    }

    public T5 Param5
    {
        get => _param5;
        set => _param5 = value;
    }

    public T6 Param6
    {
        get => _param6;
        set => _param6 = value;
    }

    public T7 Param7
    {
        get => _param7;
        set => _param7 = value;
    }

    public T8 Param8
    {
        get => _param8;
        set => _param8 = value;
    }

    /// <inheritdoc />
    public T Get<T>(int index)
        => index switch
        {
            0 => typeof(T) != typeof(T1) ? throw new InvalidCastException() : Unsafe.As<T1, T>(ref _param1),
            1 => typeof(T) != typeof(T2) ? throw new InvalidCastException() : Unsafe.As<T2, T>(ref _param2),
            2 => typeof(T) != typeof(T3) ? throw new InvalidCastException() : Unsafe.As<T3, T>(ref _param3),
            3 => typeof(T) != typeof(T4) ? throw new InvalidCastException() : Unsafe.As<T4, T>(ref _param4),
            4 => typeof(T) != typeof(T5) ? throw new InvalidCastException() : Unsafe.As<T5, T>(ref _param5),
            5 => typeof(T) != typeof(T6) ? throw new InvalidCastException() : Unsafe.As<T6, T>(ref _param6),
            6 => typeof(T) != typeof(T7) ? throw new InvalidCastException() : Unsafe.As<T7, T>(ref _param7),
            7 => typeof(T) != typeof(T8) ? throw new InvalidCastException() : Unsafe.As<T8, T>(ref _param8),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

    /// <inheritdoc />
    public void Set<T>(int index, T value)
    {
        switch (index)
        {
            case 0:
                if (typeof(T) != typeof(T1)) throw new InvalidCastException();
                Unsafe.As<T1, T>(ref _param1) = value;
                return;
            case 1:
                if (typeof(T) != typeof(T2)) throw new InvalidCastException();
                Unsafe.As<T2, T>(ref _param2) = value;
                return;
            case 2:
                if (typeof(T) != typeof(T3)) throw new InvalidCastException();
                Unsafe.As<T3, T>(ref _param3) = value;
                return;
            case 3:
                if (typeof(T) != typeof(T4)) throw new InvalidCastException();
                Unsafe.As<T4, T>(ref _param4) = value;
                return;
            case 4:
                if (typeof(T) != typeof(T5)) throw new InvalidCastException();
                Unsafe.As<T5, T>(ref _param5) = value;
                return;
            case 5:
                if (typeof(T) != typeof(T6)) throw new InvalidCastException();
                Unsafe.As<T6, T>(ref _param6) = value;
                return;
            case 6:
                if (typeof(T) != typeof(T7)) throw new InvalidCastException();
                Unsafe.As<T7, T>(ref _param7) = value;
                return;
            case 7:
                if (typeof(T) != typeof(T8)) throw new InvalidCastException();
                Unsafe.As<T8, T>(ref _param8) = value;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}