namespace Flow.Automation.Decisioning.Abstractions;

/// <summary>
/// An enum to represent logical operators
/// </summary>
/// <remarks>
/// By using enum combinations,
/// different operators produced by corresponding bit operations can be obtained
/// </remarks>
[Flags]
public enum LogicalOperator
{
    /// <summary>Do nothing</summary>
    None = 0b_0000_0000,
    
    /// <summary>And: 0001</summary>
    And  = 0b_0000_0001,
    /// <summary>Or: 0010</summary>
    Or   = 0b_0000_0010,
    /// <summary>Not: 0100, Unitary operators, using it alone can lead to errors</summary>
    Not  = 0b_0000_0100,
    
    /// <summary>Nand: 0101</summary>
    Nand = 0b_0000_0101,
    /// <summary>Nor: 0110</summary>
    Nor  = 0b_0000_0110,
    /// <summary>Xor: 1000</summary>
    Xor  = 0b_0000_1000,
}