using System;
using Microsoft.CodeAnalysis;

namespace Flow.SDK.Plugins.Generators;

#pragma warning disable RS1038
[Generator]
#pragma warning restore RS1038
public class InternalNodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}