using System.Linq;
using System.Collections.Generic;
using Xunit;
using IdentityServer4.Models;
using FluentAssertions;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class ClientsTests
    {
        [Fact]
        public void Get_Should_Return_Valid_Clients_Collection()
        {
            // Act
            var clients = Clients.Get();

            // Assert
            clients.Should().NotBeNull();
            clients.Should().BeAssignableTo<IEnumerable<Client>>();
            clients.Should().NotBeEmpty();
        }

        [Fact]
        public void Client_Credentials_Client_Should_Have_Valid_Configuration()
        {
            // Arrange
            var clients = Clients.Get();
            
            // Act
            var client = clients.FirstOrDefault(c => c.ClientId == "client");

            // Assert
            client.Should().NotBeNull();
            client.ClientSecrets.Should().NotBeEmpty();
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.ClientCredentials);
            client.AllowOfflineAccess.Should().BeTrue();
            client.AllowedScopes.Should().Contain(new[] { "api1", "api2", "other_api" });
        }

        [Fact]
        public void Resource_Owner_Client_Should_Have_Valid_Configuration()
        {
            // Arrange
            var clients = Clients.Get();
            
            // Act
            var client = clients.FirstOrDefault(c => c.ClientId == "roclient");

            // Assert
            client.Should().NotBeNull();
            client.ClientSecrets.Should().NotBeEmpty();
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.ResourceOwnerPassword);
            client.RefreshTokenUsage.Should().Be(TokenUsage.OneTimeOnly);
            client.AllowOfflineAccess.Should().BeTrue();
        }

        [Fact]
        public void Custom_Grant_Client_Should_Have_Valid_Configuration()
        {
            // Arrange
            var clients = Clients.Get();
            
            // Act
            var client = clients.FirstOrDefault(c => c.ClientId == "client.custom");

            // Assert
            client.Should().NotBeNull();
            client.ClientSecrets.Should().NotBeEmpty();
            client.AllowedGrantTypes.Should().Contain(new[] { "custom", "custom.nosubject" });
            client.AllowOfflineAccess.Should().BeTrue();
        }
    }
}
