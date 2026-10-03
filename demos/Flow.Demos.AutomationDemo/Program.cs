using Microsoft.Extensions.Hosting;
using Flow.App.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services =>
{
    services.AddHostedService<ListenerService>();
});

var host = builder.Build();
host.Run();