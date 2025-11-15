using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

public interface IVariableConnection
{
    VariablePosition From { get; set; }

    VariablePosition To { get; set; }

    Type VariableType { get; set; }

    object? Value { get; set; }
}
