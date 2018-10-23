using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Common.Configuration;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder AddExternalConfigSources(this IWebHostBuilder hostBuilder, LoggerFactory loggerFactory = null)
        {
            return hostBuilder.ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddConfigServer(builderContext.HostingEnvironment, loggerFactory);

                    var baseConfig = config.Build();
                    if (Convert.ToBoolean(baseConfig["UseVault"]))
                    {
                        var configurationBuilder = new ConfigurationBuilder();
                        configurationBuilder.AddAzureKeyVault(
                            $"https://{baseConfig["Vault:Name"]}.vault.azure.net/",
                            baseConfig["Vault:ClientId"],
                            baseConfig["Vault:ClientSecret"]);
                        configurationBuilder.AddEnvironmentVariables();
                        config.AddConfiguration(configurationBuilder.Build());
                    }

                    config.AddInMemoryCollection(PropertyPlaceholderHelper.GetResolvedConfigurationPlaceholders(config.Build(), loggerFactory?.CreateLogger("Steeltoe.Configuration.PropertyPlaceholderHelper")));
                });
        }
    }
}
