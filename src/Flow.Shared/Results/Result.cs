namespace Flow.Shared.Results;

/// <summary>
/// An error-handling method used as an alternative to throwing exceptions,
/// which records the results and status of execution.
/// </summary>
/// <param name="IsCompleted"></param>
/// <param name="IsSuccess"></param>
/// <param name="Exception"></param>
/// <param name="Message"></param>
public record Result(
    bool IsCompleted, bool IsSuccess, Exception? Exception = null, string? Message = null)
{
    public readonly bool IsCompleted = IsCompleted;

    public readonly bool IsSuccess = IsSuccess;
    
    public readonly Exception? Exception = Exception;

    public readonly string? Message = Message ?? Exception?.Message;
    
    /// <summary>
    /// Result used to indicate “completion”.
    /// </summary>
    /// <remarks>Note that this does not indicate the task has been successfully executed.</remarks>
    public static readonly Result Completed = new(true, false);
    
    /// <summary>
    /// Result indicating “successfully completed”
    /// </summary>
    public static readonly Result Success = new(true, true);
    
    /// <summary>
    /// Result indicating “an error has occurred” with an empty exception and an empty message.
    /// </summary>
    public static readonly Result Error = new(false, false);
}