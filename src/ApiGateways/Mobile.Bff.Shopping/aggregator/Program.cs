using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator
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
            WebHost
                .CreateDefaultBuilder(args)
                .AddExternalConfigSources(logfactory)
                .ConfigureLogging((ctx, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConfiguration(ctx.Configuration);
                    builder.AddDynamicConsole();
                    builder.AddDebug();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
