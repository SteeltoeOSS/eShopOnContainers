using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace eShopConContainers.WebSPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerFactory logFactory = new LoggerFactory();
            logFactory.AddConsole(new ConsoleLoggerSettings { DisableColors = true, Switches = new Dictionary<string, LogLevel> { { "Default", LogLevel.Information } } });

            BuildWebHost(args, logFactory).Run();
        }

        public static IWebHost BuildWebHost(string[] args, LoggerFactory logfactory) =>
            WebHost.CreateDefaultBuilder(args)
             .UseStartup<Startup>()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
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
