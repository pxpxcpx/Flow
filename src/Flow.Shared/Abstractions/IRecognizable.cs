namespace Flow.Shared.Abstractions;

public interface IRecognizable<TId> 
{
    TId Identifier { get; set; }
}