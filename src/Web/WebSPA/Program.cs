using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Logging;

namespace eShopConContainers.WebSPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .UseStartup<Startup>()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddDynamicConsole();
                    builder.AddDebug();
                })
                .UseApplicationInsights()                
                .Build();       
    }
}
