using Flow.Shared.Models;

namespace Flow.Shared.Exceptions;

public class ValuePassingException : Exception
{
    public readonly int ScriptId;

    public readonly int Step;

    public readonly Wrapper? Detail;

    public ValuePassingException()
    {
    }

    public ValuePassingException(string message, int scriptId = 0, int step = 0, Wrapper? detail = null)
        : base(message)
    {
        ScriptId = scriptId;
        Step = step;
        Detail = detail;
    }
}