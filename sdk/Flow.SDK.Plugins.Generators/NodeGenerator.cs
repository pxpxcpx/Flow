using System;
using Microsoft.CodeAnalysis;

namespace Flow.SDK.Plugins.Generators;

[Generator]
public class InternalNodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}