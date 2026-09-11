namespace Flow.Shared.Utils;

public static class FlagExtensions
{
    public static bool IsOnlyOneFlagIn<TFlag>(this TFlag flag, TFlag mask)
        where TFlag : Enum, IConvertible
    {
        var f = flag.ToInt32(null) | mask.ToInt32(null);
        return (f & (f - 1)) == 0;
    }

    public static int ExtractFieldIn<TFlag>(this TFlag flag, TFlag mask)
        where TFlag : Enum, IConvertible
        => flag.ToInt32(null) & mask.ToInt32(null);

    public static TFlag ExtractFieldIn<TFlag>(this TFlag flag, TFlag mask, Func<int, TFlag> extractor)
        where TFlag : Enum, IConvertible
        => extractor.Invoke(flag.ExtractFieldIn(mask));
}