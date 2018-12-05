using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IHttpClientBuilderExtensions
    {

        /// <summary>
        /// Easily disable certificate validation for any HTTP call used by a client from a given HttpClientFactory
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validateCertificates"></param>
        /// <returns></returns>
        public static IHttpClientBuilder SetCertificateValidation(this IHttpClientBuilder builder, bool validateCertificates)
        {
            if (!validateCertificates)
            {
                builder.ConfigureHttpMessageHandlerBuilder(handlerBuilder => handlerBuilder.PrimaryHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true });
            }

            return builder;
        }
    }
}
