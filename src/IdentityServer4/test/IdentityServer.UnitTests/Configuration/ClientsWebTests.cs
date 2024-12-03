using System.Linq;
using FluentAssertions;
using IdentityServer4;
using IdentityServerHost.Configuration;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class ClientsWebTests
    {
        [Fact]
        public void Get_Should_Return_Expected_Clients()
        {
            // Act
            var clients = ClientsWeb.Get().ToList();

            // Assert
            clients.Should().HaveCount(4);
            
            // Verify JS OIDC Client
            var jsClient = clients.Single(x => x.ClientId == "js_oidc");
            jsClient.RequireClientSecret.Should().BeFalse();
            jsClient.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.Code);
            
            // Verify MVC Token Management Client
            var mvcTokenClient = clients.Single(x => x.ClientId == "mvc.tokenmanagement");
            mvcTokenClient.RequirePkce.Should().BeTrue();
            mvcTokenClient.AccessTokenLifetime.Should().Be(75);
            
            // Verify MVC Code Flow Client
            var mvcCodeClient = clients.Single(x => x.ClientId == "mvc.code");
            mvcCodeClient.RequireConsent.Should().BeTrue();
            mvcCodeClient.AllowOfflineAccess.Should().BeTrue();
            
            // Verify MVC Hybrid Client
            var mvcHybridClient = clients.Single(x => x.ClientId == "mvc.hybrid.backchannel");
            mvcHybridClient.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.Hybrid);
            mvcHybridClient.RequirePkce.Should().BeFalse();
            mvcHybridClient.BackChannelLogoutUri.Should().Be("https://localhost:44303/logout");
        }

        [Fact]
        public void All_Clients_Should_Have_Valid_Scopes()
        {
            // Act
            var clients = ClientsWeb.Get();

            // Assert
            foreach (var client in clients)
            {
                client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.OpenId);
                client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.Profile);
                client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.Email);
                client.AllowedScopes.Should().Contain("resource1.scope1");
                client.AllowedScopes.Should().Contain("resource2.scope1");
                client.AllowedScopes.Should().Contain("transaction");
            }
        }
    }
}
