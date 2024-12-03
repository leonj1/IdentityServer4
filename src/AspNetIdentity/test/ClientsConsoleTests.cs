using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServerHost.Configuration;
using Xunit;

namespace IdentityServer4.AspNetIdentity.Tests
{
    public class ClientsConsoleTests
    {
        [Fact]
        public void Get_ShouldReturnExpectedNumberOfClients()
        {
            // Act
            var clients = ClientsConsole.Get();

            // Assert
            Assert.NotNull(clients);
            Assert.Equal(12, clients.Count());
        }

        [Fact]
        public void ClientCredentialsClient_ShouldHaveCorrectConfiguration()
        {
            // Act
            var client = ClientsConsole.Get().First(c => c.ClientId == "client");

            // Assert
            Assert.NotNull(client);
            Assert.Single(client.ClientSecrets);
            Assert.Equal(GrantTypes.ClientCredentials, client.AllowedGrantTypes);
            Assert.Contains("resource1.scope1", client.AllowedScopes);
            Assert.Contains("resource2.scope1", client.AllowedScopes);
            Assert.Contains(IdentityServerConstants.LocalApi.ScopeName, client.AllowedScopes);
        }

        [Fact]
        public void DeviceFlowClient_ShouldHaveCorrectConfiguration()
        {
            // Act
            var client = ClientsConsole.Get().First(c => c.ClientId == "device");

            // Assert
            Assert.NotNull(client);
            Assert.Equal("Device Flow Client", client.ClientName);
            Assert.Equal(GrantTypes.DeviceFlow, client.AllowedGrantTypes);
            Assert.False(client.RequireClientSecret);
            Assert.True(client.AllowOfflineAccess);
            Assert.Contains(IdentityServerConstants.StandardScopes.OpenId, client.AllowedScopes);
            Assert.Contains(IdentityServerConstants.StandardScopes.Profile, client.AllowedScopes);
            Assert.Contains(IdentityServerConstants.StandardScopes.Email, client.AllowedScopes);
        }

        [Fact]
        public void ConsolePkceClient_ShouldHaveCorrectConfiguration()
        {
            // Act
            var client = ClientsConsole.Get().First(c => c.ClientId == "console.pkce");

            // Assert
            Assert.NotNull(client);
            Assert.Equal("Console with PKCE Sample", client.ClientName);
            Assert.False(client.RequireClientSecret);
            Assert.True(client.RequirePkce);
            Assert.Contains("http://127.0.0.1", client.RedirectUris);
            Assert.True(client.AllowOfflineAccess);
        }
    }
}
