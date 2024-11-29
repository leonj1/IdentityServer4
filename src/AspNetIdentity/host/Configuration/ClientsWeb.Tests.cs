using System.Linq;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServerHost.Configuration.Tests
{
    public class ClientsWebTests
    {
        [Fact]
        public void Get_ReturnsExpectedNumberOfClients()
        {
            var clients = ClientsWeb.Get().ToList();
            Assert.Equal(4, clients.Count);
        }

        [Fact]
        public void JsOidcClient_HasCorrectConfiguration()
        {
            var client = ClientsWeb.Get().First(c => c.ClientId == "js_oidc");
            
            Assert.Equal("JavaScript OIDC Client", client.ClientName);
            Assert.Equal("http://identityserver.io", client.ClientUri);
            Assert.False(client.RequireClientSecret);
            Assert.Contains("https://localhost:44300/index.html", client.RedirectUris);
            Assert.Contains("https://localhost:44300", client.AllowedCorsOrigins);
        }

        [Fact]
        public void MvcTokenManagementClient_HasCorrectConfiguration()
        {
            var client = ClientsWeb.Get().First(c => c.ClientId == "mvc.tokenmanagement");
            
            Assert.True(client.RequirePkce);
            Assert.Equal(75, client.AccessTokenLifetime);
            Assert.True(client.AllowOfflineAccess);
            Assert.Contains("https://localhost:44301/signin-oidc", client.RedirectUris);
        }

        [Fact]
        public void MvcCodeClient_HasCorrectConfiguration()
        {
            var client = ClientsWeb.Get().First(c => c.ClientId == "mvc.code");
            
            Assert.Equal("MVC Code Flow", client.ClientName);
            Assert.True(client.RequireConsent);
            Assert.True(client.AllowOfflineAccess);
            Assert.Contains("https://localhost:44302/signin-oidc", client.RedirectUris);
        }

        [Fact]
        public void MvcHybridClient_HasCorrectConfiguration()
        {
            var client = ClientsWeb.Get().First(c => c.ClientId == "mvc.hybrid.backchannel");
            
            Assert.Equal("MVC Hybrid (with BackChannel logout)", client.ClientName);
            Assert.False(client.RequirePkce);
            Assert.True(client.AllowOfflineAccess);
            Assert.Contains("https://localhost:44303/logout", client.BackChannelLogoutUri);
        }

        [Fact]
        public void AllClients_HaveRequiredScopes()
        {
            var requiredScopes = new[]
            {
                "openid",
                "profile", 
                "email",
                "resource1.scope1",
                "resource2.scope1",
                "transaction"
            };

            foreach (var client in ClientsWeb.Get())
            {
                foreach (var scope in requiredScopes)
                {
                    Assert.Contains(scope, client.AllowedScopes);
                }
            }
        }
    }
}
