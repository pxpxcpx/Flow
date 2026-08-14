using System.Reactive.Subjects;
using Flow.Automation.Messaging.Abstractions;
using Flow.Shared.Models;

namespace Flow.Automation.Messaging.Components;

public sealed class SubjectWrapper<TMessage> : Wrapper, ISubjectWrapper
{
    private Subject<TMessage> _subject;

    public SubjectWrapper(Subject<TMessage> subject)
        : base(subject)
    {
        _subject = subject;
    }

    public SubjectWrapper(Type type, Subject<TMessage> subject) 
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