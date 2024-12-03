using System.Linq;
using FluentAssertions;
using IdentityServer4;
using IdentityServerHost.Configuration;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class ClientsConsoleTests
    {
        [Fact]
        public void Get_Should_Return_Expected_Clients()
        {
            var clients = ClientsConsole.Get().ToList();
            
            clients.Should().NotBeEmpty();
            clients.Count.Should().Be(11);
        }

        [Fact]
        public void Client_Credentials_Flow_Client_Should_Be_Configured_Correctly()
        {
            var client = ClientsConsole.Get()
                .First(x => x.ClientId == "client");

            client.ClientId.Should().Be("client");
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.ClientCredentials);
            client.ClientSecrets.Count.Should().Be(1);
            client.AllowedScopes.Should().Contain("resource1.scope1");
            client.AllowedScopes.Should().Contain("resource2.scope1");
            client.AllowedScopes.Should().Contain(IdentityServerConstants.LocalApi.ScopeName);
        }

        [Fact]
        public void Device_Flow_Client_Should_Be_Configured_Correctly()
        {
            var client = ClientsConsole.Get()
                .First(x => x.ClientId == "device");

            client.ClientId.Should().Be("device");
            client.ClientName.Should().Be("Device Flow Client");
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.DeviceFlow);
            client.RequireClientSecret.Should().BeFalse();
            client.AllowOfflineAccess.Should().BeTrue();
            client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.OpenId);
            client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.Profile);
            client.AllowedScopes.Should().Contain(IdentityServerConstants.StandardScopes.Email);
        }

        [Fact]
        public void Resource_Owner_Password_Client_Should_Be_Configured_Correctly()
        {
            var client = ClientsConsole.Get()
                .First(x => x.ClientId == "roclient");

            client.ClientId.Should().Be("roclient");
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.ResourceOwnerPassword);
            client.AllowOfflineAccess.Should().BeTrue();
            client.RefreshTokenUsage.Should().Be(TokenUsage.OneTimeOnly);
            client.AbsoluteRefreshTokenLifetime.Should().Be(3600 * 24);
            client.SlidingRefreshTokenLifetime.Should().Be(10);
            client.RefreshTokenExpiration.Should().Be(TokenExpiration.Sliding);
        }
    }
}
