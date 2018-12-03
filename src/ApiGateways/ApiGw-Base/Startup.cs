using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.eShopOnContainers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Pivotal.Discovery.Client;
using Steeltoe.Management.CloudFoundry;
using System;
using System.Threading.Tasks;

namespace OcelotApiGw
{
    public class Startup
    {

        private readonly IConfiguration _cfg;

        public Startup(IConfiguration configuration)
        {
            _cfg = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscoveryClient(_cfg);
            var identityServerUrl = services.GetExternalIdentityUrl();
            Console.WriteLine("Using {0} as the identity server url", identityServerUrl);
            var authenticationProviderKey = "IdentityApiKey";

            // app.Use a custom middleware that basically does this down below...
            // services.AddCors(options =>
            // {
            //     options.AddPolicy("CorsPolicy",
            //         builder => builder.AllowAnyOrigin()
            //         .AllowAnyMethod()
            //         .AllowAnyHeader()
            //         .AllowCredentials());
            // });

            // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(authenticationProviderKey, x =>
                {
                    x.Authority = identityServerUrl;
                    x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudiences = new[] { "orders", "basket", "locations", "marketing", "mobileshoppingagg", "webshoppingagg" }
                    };
                    x.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = async ctx =>
                        {
                            await Task.Run(() => Console.WriteLine("OnAuthenticationFailed: {0}", ctx.Exception));
                        },
                        OnTokenValidated = async ctx =>
                        {
                            int i = 0;
                        },
                        OnMessageReceived = async ctx =>
                        {
                            int i = 0;
                        }
                    };
                });
            services.AddCloudFoundryActuators(_cfg);
            services.AddOcelot(_cfg)
                .AddEureka();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = _cfg["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedProto });

            app.Use(async (context, next) =>
            {
                // explicitly allow [just about] anything for CORS, but return the origin of the request instead of a wildcard
                //  - because SignalR won't work with a wildcard response anymore (because accepting requests with credentials from anywhere introduces risk)
                //  - an alternative method would be to use Eureka to get the specific hosts that should be initiating requests
                SetCorsHeaderIfNotAlreadySet(context, "Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                SetCorsHeaderIfNotAlreadySet(context, "Access-Control-Allow-Headers", context.Request.Headers["Access-Control-Request-Headers"].ToString());
                SetCorsHeaderIfNotAlreadySet(context, "Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, HEAD");
                SetCorsHeaderIfNotAlreadySet(context, "Access-Control-Allow-Credentials", "true");

                await next();
            });

            // app.UseCors("CorsPolicy");
            app.UseCloudFoundryActuators();
            app.UseWebSockets();
            app.UseOcelot().Wait();
        }

        private void SetCorsHeaderIfNotAlreadySet(HttpContext context, string headerName, string headerValue)
        {
            if (!context.Response.Headers.ContainsKey(headerName) && !string.IsNullOrEmpty(headerValue))
            {
                context.Response.Headers.Add(headerName, headerValue);
            }
        }
    }
}
