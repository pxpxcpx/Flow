using Flow.Shared.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Flow.Shared.Abstractions;

public interface IPlugin : IPluggable
{
    PluginMetadata Metadata { get; }
    
    HostBuilderContext HostBuilderContext { get; }
    
    IServiceCollection Services { get; }
}