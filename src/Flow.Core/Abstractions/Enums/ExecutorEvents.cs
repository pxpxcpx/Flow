namespace Flow.Core.Abstractions.Enums;

public enum ExecutorEvents
{
    None = 0,
    
    //
    // Lifecycle events:
    // bits controlled range: 1-8
    //
    LifecycleMask     = 0b_0000_1111, // For internal ues only.
    Initialize  = 1, // 0b_0000_0001, 1
    Start       = 2, // 0b_0000_0010, 2
    Suspend     = 3, // 0b_0000_0011, 3
    Pause       = 4, // 0b_0000_0100, 4
    Resume      = 5, // 0b_0000_0101, 5
    Cancel      = 6, // 0b_0000_0110, 6
    Complete    = 7, // 0b_0000_0111, 7
    Reset       = 8, // 0b_0000_1000, 8
    
    //
    // Execution events:
    // bits controlled range: 16-48
    //
    ResultMask         = 0b_1111_0000, // For internal ues only.
    Unspecified = 16, // 0b_0001_0000, 16
    Success     = 32, // 0b_0010_0000, 17
    Fail        = 48, // 0b_0011_0000, 18
}