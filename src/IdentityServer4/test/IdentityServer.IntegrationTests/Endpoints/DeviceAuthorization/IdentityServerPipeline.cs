using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Common
{
    public class IdentityServerPipeline
    {
        public const string DeviceAuthorization = "/connect/deviceauthorization";

        private List<Client> _clients = new List<Client>();
        private List<IdentityResource> _identityScopes = new List<IdentityResource>();

        public HttpClient BackChannelClient { get; } = new HttpClient();

        public void Initialize()
        {
            foreach (var client in _clients)
            {
                // Add client to IdentityServer
            }

            foreach (var identityScope in _identityScopes)
            {
                // Add identity scope to IdentityServer
            }
        }

        public void Clients(List<Client> clients)
        {
            _clients.AddRange(clients);
        }

        public void IdentityScopes(List<IdentityResource> identityScopes)
        {
            _identityScopes.AddRange(identityScopes);
        }
    }
}
