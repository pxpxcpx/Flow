using Flow.Shared.Metadata;

namespace Flow.Shared.Models;

public abstract class Plugin : IDisposable
{
    private bool _disposed;

    public abstract PluginMetadata Metadata { get; }
    
    public abstract object?[]? Dependencies { get; set; }

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
