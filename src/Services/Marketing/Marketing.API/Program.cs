using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Marketing.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerFactory logFactory = new LoggerFactory();
            logFactory.AddConsole(new ConsoleLoggerSettings { DisableColors = true, Switches = new Dictionary<string, LogLevel> { { "Default", LogLevel.Information } } });

            BuildWebHost(args, logFactory)
                .MigrateDbContext<MarketingContext>((context, services) =>
                {
                    var logger = services.GetService<ILogger<MarketingContextSeed>>();

                    new MarketingContextSeed()
                        .SeedAsync(context,logger)
                        .Wait();

                }).Run();
        }

        public static IWebHost BuildWebHost(string[] args, LoggerFactory logfactory) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseWebRoot("Pics")
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
