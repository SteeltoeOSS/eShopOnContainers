using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Common.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers
{
    /// <summary>
    /// Identity server needs to know where its clients 
    /// </summary>
    public static class PostStartupClientUrlUpdater
    {
        public static IWebHost UpdateIdentityClientsFromEureka(this IWebHost webHost, List<string> clientsToUpdate, LoggerFactory loggerFactory)
        {
            bool clientsIterated = false;
            int iterationCount = 0;
            Task.Run(() => 
            {
                using (var scope = webHost.Services.CreateScope())
                {
                    var _discoveryClient = scope.ServiceProvider.GetService<IDiscoveryClient>();

                    var _logger = loggerFactory.CreateLogger("Microsoft.eShopOnContainers.PostStartupClientUrlUpdater");
                    var updatedClients = new List<string>();

                    while (clientsToUpdate.Count > updatedClients.Count)
                    {
                        _logger.LogDebug("Entering client update loop, {0} clients remaining", clientsToUpdate.Count - updatedClients.Count);
                        var _context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                        var clients = _context.Clients
                            .Include(r => r.RedirectUris)
                            .Include(p => p.PostLogoutRedirectUris);

                        if (!clientsIterated)
                        {
                            foreach (var c in clients)
                            {
                                _logger.LogTrace("Found client: {0} with uri {1}", c.ClientId, c.ClientUri);
                            }

                            clientsIterated = true;
                        }

                        // for each of the clients to update where the client hasn't already been updated...
                        foreach (var app in clientsToUpdate.Where(c => !updatedClients.Contains(c)))
                        {
                            if (!_discoveryClient.GetInstances(app).Any())
                            {
                                _logger.LogDebug("Discovery client didn't find {0} registered yet", app);
                            }
                            else
                            {
                                _logger.LogTrace("Discovery client found an entry for {0}", app);
                                var serviceEntry = _discoveryClient.GetInstances(app).FirstOrDefault();
                                var externalUrl = serviceEntry.Metadata["externalUrl"];
                                if (!externalUrl.StartsWith("http")){
                                    if (app.StartsWith("web", StringComparison.InvariantCultureIgnoreCase) || app.StartsWith("mobile", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        externalUrl = "https://" + externalUrl;
                                    }
                                    else
                                    {
                                        externalUrl = "http://" + externalUrl;
                                    }
                                }

                                var toUpdate = clients.First(n => n.ClientId.Equals(app));
                                if (!(toUpdate is null))
                                {
                                    if (toUpdate.ClientUri != externalUrl)
                                    {
                                        _logger.LogDebug("Updating {0} to {1}", toUpdate.ClientUri, externalUrl);
                                        foreach (var u in toUpdate.RedirectUris.Where(u => u.RedirectUri.StartsWith(toUpdate.ClientUri)))
                                        {
                                            _logger.LogTrace("Updating RedirectUri {0}, replacing {1} with {2}", u.RedirectUri, toUpdate.ClientUri, externalUrl);
                                            u.RedirectUri = u.RedirectUri.Replace(toUpdate.ClientUri, externalUrl);
                                        }
                                        foreach (var u in toUpdate.PostLogoutRedirectUris.Where(u => u.PostLogoutRedirectUri.StartsWith(toUpdate.ClientUri)))
                                        {
                                            _logger.LogTrace("Updating PostLogoutRedirectUri {0}, replacing {1} with {2}", u.PostLogoutRedirectUri, toUpdate.ClientUri, externalUrl);
                                            u.PostLogoutRedirectUri = u.PostLogoutRedirectUri.Replace(toUpdate.ClientUri, externalUrl);
                                        }

                                        toUpdate.ClientUri = externalUrl;
                                    }
                                    else
                                    {
                                        _logger.LogDebug("Client {ClientId} already up to date with externalUrl {externalUrl}", toUpdate.ClientId, externalUrl);
                                    }
                                }
                                else
                                {
                                    _logger.LogTrace("Could not find identity client {0} to update, this is unexpected!", app);
                                }

                                updatedClients.Add(app);
                            }
                        }

                        _context.SaveChanges();
                        Thread.Sleep(iterationCount * 1500 + 5000);
                        iterationCount++;
                    }

                    _logger.LogTrace("Finished updating clients");
                }
            });

            return webHost;
       }
    }
}
