using bidService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Web;

// Set up NLog logger using configuration from app settings
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    // Create a default host builder
    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddMemoryCache();
            services.AddHostedService<Worker>();
        })
        .UseNLog()
        .Build();
    host.Run();
}
catch (Exception ex)
{
    // Log the exception and throw it again
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}