using Microsoft.Extensions.Logging;
using Steeltoe.Common.Discovery;
using System.Linq;
using System.Threading;

namespace Microsoft.eShopOnContainers
{
    public static class DiscoveryClientExtensions
    {
        /// <summary>
        /// Return the property 'externalUrl' from the metadata of an app in the service registry
        /// </summary>
        /// <param name="client"></param>
        /// <param name="appName">The name of the application to look up</param>
        /// <returns>An externally-accessible address an application believes itself to be available on</returns>
        /// <remarks>Waits for the service to be registered, with a 5 second delay between requests to the registry</remarks>
        public static string GetExternalUrlForApplication(this IDiscoveryClient client, string appName, ILogger logger)
        {
            while (!client.GetInstances(appName).Any())
            {
                logger.LogWarning("Discovery client didn't find {0} registered, trying again shortly", appName);
                Thread.Sleep(5000);
            }
            var serviceEntry = client.GetInstances(appName).FirstOrDefault();

            return serviceEntry.Metadata["externalUrl"].StartsWith("http") 
                    ? serviceEntry.Metadata["externalUrl"] 
                    : $"https://{serviceEntry.Metadata["externalUrl"]}";
        }
    }
}
