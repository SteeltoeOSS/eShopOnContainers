using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Common.Configuration;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace Payment.API
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
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddConfigServer(builderContext.HostingEnvironment, logfactory);
                    config.AddInMemoryCollection(PropertyPlaceholderHelper.GetResolvedConfigurationPlaceholders(config.Build(), logfactory?.CreateLogger("PropertyPlaceholderHelper")));
                })
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
