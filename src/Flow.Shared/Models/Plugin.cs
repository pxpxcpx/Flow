using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Flow.Shared.Models;

public abstract class Plugin : IPlugin
{
    private bool _disposed;

    public abstract PluginMetadata Metadata { get; }
    
    public abstract object?[]? Dependencies { get; set; }
    
    public HostBuilderContext HostBuilderContext { get; } 
    
    public IServiceCollection Services { get; set; }

    public abstract Task Initialize();

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // TODO: Release unmanaged resources here.

        _disposed = true;
    }

    public void Dispose()
    {
       Dispose(true);
       GC.SuppressFinalize(this);
    }
}
