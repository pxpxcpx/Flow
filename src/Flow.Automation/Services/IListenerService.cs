using Microsoft.Extensions.Hosting;

namespace Flow.Automation.Services;

public interface IListenerService : IDisposable, IHostedService;
