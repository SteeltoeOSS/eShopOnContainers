using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerFactory logFactory = new LoggerFactory();
            logFactory.AddConsole(new ConsoleLoggerSettings { DisableColors = true, Switches = new Dictionary<string, LogLevel> { { "Default", LogLevel.Information } } });
            BuildWebHost(args, logFactory).Run();
        }

        public static IWebHost BuildWebHost(string[] args, LoggerFactory logfactory)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s => s.AddSingleton(builder))
                .ConfigureAppConfiguration(ic => {
                    ic.AddJsonFile(Path.Combine("configuration", "configuration.json"));
                })
                .AddExternalConfigSources(logfactory)
                .ConfigureLogging((ctx, loggingbuilder) =>
                {
                    loggingbuilder.ClearProviders();
                    loggingbuilder.AddConfiguration(ctx.Configuration);
                    loggingbuilder.AddDynamicConsole();
                    loggingbuilder.AddDebug();
                })
                .UseCloudFoundryHosting()
                .UseStartup<Startup>();
            IWebHost host = builder.Build();
            return host;
        }
    }
}
