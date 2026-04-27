using Flow.SDK.Utils;

namespace Flow.SDK.Abstractions;

public interface II18NRequired
{
    static abstract I18NHelper I18NHelper { get; }
}