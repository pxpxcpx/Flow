using Flow.Shared.Abstractions;

namespace Flow.Automation.Messaging;

public interface ISubjectContainer : ITypedObject
{
    void OnNext(object value);
    
    void OnError(Exception error);
    
    void OnCompleted();

    void Dispose();
}