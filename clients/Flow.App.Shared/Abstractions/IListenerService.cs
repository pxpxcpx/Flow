using Microsoft.Extensions.Hosting;

namespace Flow.App.Shared.Abstractions;

public interface IListenerService : IDisposable, IHostedService;
