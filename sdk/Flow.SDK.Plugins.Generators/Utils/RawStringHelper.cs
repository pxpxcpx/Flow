using System.Linq;

namespace Flow.SDK.Plugins.Generators.Utils;

internal static class RawStringHelper
{
    public static string AlignWithIndent(this string rawString, int indent)
    {
        var i = new string(' ', indent);

        return string.Join("\n", 
            rawString.Split('\n').Select(line => i + line));
    }
}
