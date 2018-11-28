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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

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

            app.UseCors("CorsPolicy");
            app.UseCloudFoundryActuators();
            app.UseOcelot().Wait();
        }
    }
}
