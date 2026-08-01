using Flow.Shared.Abstractions;

namespace Flow.Automation.Messaging.Abstractions;

public interface ISubjectWrapper : ITypedObject
{
    void OnNext(object value);
    
    void OnError(Exception error);
    
    void OnCompleted();

    void Dispose();
}