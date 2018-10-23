using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Logging;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Locations.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerFactory logFactory = new LoggerFactory();
            logFactory.AddConsole(minLevel: LogLevel.Trace);

            BuildWebHost(args, logFactory).Run();
        }

        public static IWebHost BuildWebHost(string[] args, LoggerFactory logfactory) =>
            WebHost.CreateDefaultBuilder(args)
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .AddExternalConfigSources(logfactory)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddDynamicConsole();
                    builder.AddDebug();
                })
                .UseApplicationInsights()
                .Build();
    }
}
