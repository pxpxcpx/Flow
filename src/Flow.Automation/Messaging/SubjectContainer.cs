using System.Reactive.Subjects;
using Flow.Shared.Models;

namespace Flow.Automation.Messaging;

public class SubjectContainer<TMessage> : TypedObject, ISubjectContainer
{
    private Subject<TMessage> _subject;

    public SubjectContainer(Subject<TMessage> subject)
        : base(subject)
    {
        _subject = subject;
    }

    public SubjectContainer(Type type, Subject<TMessage> subject) 
        : base(type, subject)
    {
        _subject = subject;
    }
    
    public void OnNext(object value) 
        => _subject.OnNext((TMessage)value);

    public void OnError(Exception error)
        => _subject.OnError(error);

    public void OnCompleted()
        => _subject.OnCompleted();

    public void Dispose()
        => _subject.Dispose();
}