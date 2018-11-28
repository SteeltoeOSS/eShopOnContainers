using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Common.Discovery;

namespace Microsoft.eShopOnContainers
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Returns the external address for the identity server. Waits until it finds an address to continue
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static string GetExternalIdentityUrl(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var discoverer = sp.GetService<IDiscoveryClient>();
            return discoverer.GetExternalUrlForApplication("identityapi", sp.GetService<ILogger<IDiscoveryClient>>());
        }
    }
}
