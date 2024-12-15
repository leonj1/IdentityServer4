using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace IdentityServer.IntegrationTests.Common
{
    public class IdentityServerPipeline
    {
        public static readonly string TokenEndpoint = "/connect/token";

        public BackChannelClient BackChannelClient { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<TestUser> Users { get; set; } = new List<TestUser>();
        public List<IdentityResource> IdentityScopes { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public List<ApiScope> ApiScopes { get; set; } = new List<ApiScope>();

        public void Initialize()
        {
            BackChannelClient = new BackChannelClient
            {
                BaseAddress = "http://127.0.0.1:5000"
            };
        }
    }
}
