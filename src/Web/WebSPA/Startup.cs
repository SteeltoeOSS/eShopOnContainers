﻿using eShopOnContainers.WebSPA;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Pivotal.Discovery.Client;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Security.DataProtection;
using System;
using System.IO;
using WebSPA.Infrastructure;

namespace eShopConContainers.WebSPA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment _hostingEnv;
        public Startup(IHostingEnvironment env)
        {
            _hostingEnv = env;

            var localPath = new Uri(Configuration["ASPNETCORE_URLS"])?.LocalPath ?? "/";
            Configuration["BaseUrl"] = localPath;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterAppInsights(services);

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }

                var healthPath = "/management/health";
                checks.AddUrlCheck(Configuration["CatalogUrl"] + healthPath, TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["OrderingUrl"] + healthPath, TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["BasketUrl"] + healthPath, TimeSpan.Zero); //No cache for this HealthCheck, better just for demos 
                checks.AddUrlCheck(Configuration["IdentityUrl"] + healthPath, TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["MarketingUrl"] + healthPath, TimeSpan.FromMinutes(minutes));

            });

            services.Configure<AppSettings>(Configuration);
            services.AddRedisConnectionMultiplexer(Configuration);

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webspa";
                })
                .PersistKeysToRedis()
                .SetApplicationName("DataProtection-Keys");
            }

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddCloudFoundryActuators(Configuration);
            services.AddDiscoveryClient(Configuration);
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IAntiforgery antiforgery)
        {

            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configure XSRF middleware, This pattern is for SPA style applications where XSRF token is added on Index page 
            // load and passed back token on every subsequent async request            
            // app.Use(async (context, next) =>
            // {
            //     if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase))
            //     {
            //         var tokens = antiforgery.GetAndStoreTokens(context);
            //         context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false });
            //     }
            //     await next.Invoke();
            // });

            //Seed Data
            WebContextSeed.Seed(app, env, loggerFactory);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            app.Use(async (context, next) =>
            {
                await next();

                // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                // Rewrite request to use app root
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) && !context.Request.Path.Value.StartsWith("/api"))
                {
                    context.Request.Path = "/index.html"; 
                    context.Response.StatusCode = 200; // Make sure we update the status code, otherwise it returns 404
                    await next();
                }
            });
            
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCloudFoundryActuators();
            app.UseDiscoveryClient();
            app.UseMvcWithDefaultRoute();
        }

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.EnableKubernetes();
            }
            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }
        }
    }
}
