using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Logging;
using System.IO;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s => s.AddSingleton(builder))
                .ConfigureAppConfiguration(ic => ic.AddJsonFile(Path.Combine("configuration", "configuration.json")))
                .ConfigureLogging((ctx, loggingbuilder) =>
                {
                    loggingbuilder.ClearProviders();
                    loggingbuilder.AddConfiguration(ctx.Configuration);
                    loggingbuilder.AddDynamicConsole();
                    loggingbuilder.AddDebug();
                })
                .UseStartup<Startup>();
            IWebHost host = builder.Build();
            return host;
        }
    }
}
