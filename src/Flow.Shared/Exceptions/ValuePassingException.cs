namespace Flow.Shared.Exceptions;

public class ValuePassingException : Exception
{
    public ValuePassingException()
    {
    }

    public ValuePassingException(string message) : base(message)
    {
    }
}