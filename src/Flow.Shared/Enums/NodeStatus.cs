namespace Flow.Shared.Enums;

[Flags]
public enum NodeStatus
{
    Ready      = 0b_0000_0000,
    Running    = 0b_0000_0001,
    Waiting    = 0b_0000_0010,
    Completed  = 0b_0000_0100,
    Failed     = 0b_0000_1000,
    Skipped    = 0b_0001_0000,
    Cancelled  = 0b_0010_0000,
}

/// <summary>
/// States of a node.
/// </summary>
[Flags]
public enum NodeStates
{
    None      = 0b_0000_0000,
    
    //
    // Lifecycle managing states:
    //
    
    LifecycleMask = 0b_0001_1111,
    
    /// <summary>
    /// Initialized state, node had just been activated.
    /// </summary>
    Idle          = 0b_0000_0001,
    
    /// <summary>
    /// Node is ready to run. All services, requirements are ready.
    /// </summary>
    Ready         = 0b_0000_0010,
    
    /// <summary>
    /// Node is being executed.
    /// </summary>
    Running       = 0b_0000_0100,
    
    /// <summary>
    /// Node has suspended due to unexpected conditions, like params are not ready, etc.
    /// </summary>
    Suspended     = 0b_0000_1000,
    
    /// <summary>
    /// Execution finished.
    /// </summary>
    Finished      = 0b_0001_0000,
    
    //
    // Execution results:
    //
    
    ResultMask  = 0b_1110_0000,
    
    /// <summary>
    /// Haven't been executed or unknown execution state (e.g. Pause or cancel during the execution).
    /// </summary>
    Unspecified = 0b_0010_0000,
    
    /// <summary>
    /// Error occured during the execution.
    /// </summary>
    Faulted     = 0b_0100_0000,
    
    /// <summary>
    /// Execution have successfully completed.
    /// </summary>
    Successful  = 0b_1000_0000,
}