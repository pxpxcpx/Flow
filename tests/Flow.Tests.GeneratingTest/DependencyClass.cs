using Flow.SDK.Attributes;
using Flow.SDK.Utils;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;

namespace Flow.Tests.GenerationTest;

[Dependency]
[I18NRequired("$$Dependency_I18N:")]
public partial class DependencyClass
{
    public override bool IsRequired { get; set; }
    
    public override DependencyMetadata Target { get; set; }
    
    public override DependencyMetadata Current { get; set; }
    
    public override DependencyConditionType ConditionType { get; set; }
    
    public override Version? MinVersion { get; set; }

    public override Task Initialize()
    {
        
        throw new NotImplementedException();
    }

    public override Predicate<(DependencyMetadata, DependencyMetadata)>? Condition { get; set; }
    
    public static I18NHelper I18NHelper { get; }
}