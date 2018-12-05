using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;

namespace Microsoft.eShopOnContainers
{
    public static class AuthenticationOptionsExtensions
    {
        public static void SetBackChannelCertificateValidation(this RemoteAuthenticationOptions options, bool validateCertificates)
        {
            if (!validateCertificates)
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
            }
        }
        public static void SetBackChannelCertificateValidation(this JwtBearerOptions options, bool validateCertificates)
        {
            if (!validateCertificates)
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
            }
        }
    }
}
