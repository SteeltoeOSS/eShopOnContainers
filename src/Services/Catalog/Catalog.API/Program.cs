using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerFactory logFactory = new LoggerFactory();
            logFactory.AddConsole(new ConsoleLoggerSettings { DisableColors = true, Switches = new Dictionary<string, LogLevel> { { "Default", LogLevel.Information } } });

            BuildWebHost(args, logFactory)
                .MigrateDbContext<CatalogContext>((context,services)=>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<CatalogSettings>>();
                    var logger = services.GetService<ILogger<CatalogContextSeed>>();

                    new CatalogContextSeed()
                    .SeedAsync(context,env,settings,logger)
                    .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_,__)=> { })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args, LoggerFactory logfactory) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("Pics")
                .AddExternalConfigSources(logfactory)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddDynamicConsole();
                    builder.AddDebug();
                })                
                .Build();    
    }
}