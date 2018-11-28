using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Configuration
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("orders", "Orders Service"),
                new ApiResource("basket", "Basket Service"),
                new ApiResource("marketing", "Marketing Service"),
                new ApiResource("locations", "Locations Service"),
                new ApiResource("mobileshoppingagg", "Mobile Shopping Aggregator"),
                new ApiResource("webshoppingagg", "Web Shopping Aggregator"),
                new ApiResource("orders.signalrhub", "Ordering Signalr Hub")
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                // JavaScript Client
                new Client
                {
                    ClientId = "WebSpa",
                    ClientName = "eShop SPA OpenId Client",
                    ClientUri = clientsUrl["WebSpa"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{clientsUrl["WebSpa"]}/" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"{clientsUrl["WebSpa"]}/" },
                    AllowedCorsOrigins = { $"{clientsUrl["WebSpa"]}" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "orders",
                        "basket",
                        "locations",
                        "marketing",
                        "webshoppingagg",
                        "orders.signalrhub"
                    },
                },
                new Client
                {
                    ClientId = "xamarin",
                    ClientName = "eShop Xamarin OpenId Client",
                    ClientUri = clientsUrl["Xamarin"],
                    AllowedGrantTypes = GrantTypes.Hybrid,                    
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { clientsUrl["Xamarin"] },
                    RequireConsent = false,
                    RequirePkce = true,
                    PostLogoutRedirectUris = { $"{clientsUrl["Xamarin"]}/Account/Redirecting" },
                    AllowedCorsOrigins = { "http://eshopxamarin" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "locations",
                        "marketing",
                        "mobileshoppingagg"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "WebMvc",
                    ClientName = "MVC Client",
                    ClientUri = clientsUrl["WebMvc"],                             // public uri of the client
                    ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string> { $"{clientsUrl["WebMvc"]}/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { $"{clientsUrl["WebMvc"]}/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "locations",
                        "marketing",
                        "webshoppingagg",
                        "orders.signalrhub"
                    },
                    AccessTokenLifetime = 60*60*2, // 2 hours
                    IdentityTokenLifetime= 60*60*2 // 2 hours
                },
                new Client
                {
                    ClientId = "mvctest",
                    ClientName = "MVC Client Test",
                    ClientUri = clientsUrl["WebMvc"],                             // public uri of the client
                    ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    RedirectUris = new List<string> { $"{clientsUrl["WebMvc"]}/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { $"{clientsUrl["WebMvc"]}/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "locations",
                        "marketing",
                        "webshoppingagg"
                    },
                },
                new Client
                {
                    ClientId = "LocationsApi",
                    ClientName = "Locations Swagger UI",
                    ClientUri = clientsUrl["LocationsApi"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["LocationsApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["LocationsApi"]}/swagger/" },

                    AllowedScopes = { "locations" }
                },
                new Client
                {
                    ClientId = "MarketingApi",
                    ClientName = "Marketing Swagger UI",
                    ClientUri = clientsUrl["MarketingApi"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["MarketingApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["MarketingApi"]}/swagger/" },

                    AllowedScopes = { "marketing" }
                },
                new Client
                {
                    ClientId = "BasketApi",
                    ClientName = "Basket Swagger UI",
                    ClientUri = clientsUrl["BasketApi"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/" },

                    AllowedScopes = { "basket" }
                },
                new Client
                {
                    ClientId = "OrderingApi",
                    ClientName = "Ordering Swagger UI",
                    ClientUri = clientsUrl["OrderingApi"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/" },

                    AllowedScopes = { "orders" }
                },
                new Client
                {
                    ClientId = "MobileShoppingAgg",
                    ClientName = "Mobile Shopping Aggregator Swagger UI",
                    ClientUri = clientsUrl["MobileShoppingAgg"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["MobileShoppingAgg"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["MobileShoppingAgg"]}/swagger/" },

                    AllowedScopes = { "mobileshoppingagg" }
                },
                new Client
                {
                    ClientId = "WebShoppingAgg",
                    ClientName = "Web Shopping Aggregator Swagger UI",
                    ClientUri = clientsUrl["WebShoppingAgg"],
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["WebShoppingAgg"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["WebShoppingAgg"]}/swagger/" },

                    AllowedScopes = { "webshoppingagg" }
                }
            };
        }
    }
}