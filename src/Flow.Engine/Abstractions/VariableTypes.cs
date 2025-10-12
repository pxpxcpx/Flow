namespace Flow.Engine.Abstractions;

[Flags]
public enum VariableTypes
{
    Input    = 0b_0000_0001,
    Output   = 0b_0000_0010,
    Required = 0b_0000_0100,
    Optional = 0b_0000_0000,
}