using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    public class ClientsTests
    {
        [Fact]
        public void Get_ShouldReturnCorrectNumberOfClients()
        {
            // Act
            var clients = Clients.Get();

            // Assert
            clients.Should().HaveCount(4);
        }

        [Theory]
        [InlineData("client1")]
        [InlineData("client2")]
        [InlineData("client3")]
        [InlineData("ro.client")]
        public void Get_ShouldReturnClientWithExpectedConfiguration(string clientId)
        {
            // Act
            var client = Clients.Get().FirstOrDefault(c => c.ClientId == clientId);

            // Assert
            client.Should().NotBeNull();
            client.ClientSecrets.Should().ContainSingle();
            client.ClientSecrets.First().Value.Should().NotBeNullOrEmpty();
            client.AllowedScopes.Should().Contain(new[] { "api1", "api2", "api3-a", "api3-b" });
            client.AccessTokenType.Should().Be(AccessTokenType.Reference);
        }

        [Fact]
        public void Get_ShouldReturnCorrectGrantTypesForStandardClients()
        {
            // Arrange
            var standardClientIds = new[] { "client1", "client2", "client3" };

            // Act
            var clients = Clients.Get().Where(c => standardClientIds.Contains(c.ClientId));

            // Assert
            clients.Should().OnlyContain(c => 
                c.AllowedGrantTypes.SequenceEqual(GrantTypes.ClientCredentials));
        }

        [Fact]
        public void Get_ShouldReturnCorrectGrantTypeForResourceOwnerClient()
        {
            // Act
            var client = Clients.Get().FirstOrDefault(c => c.ClientId == "ro.client");

            // Assert
            client.Should().NotBeNull();
            client.AllowedGrantTypes.Should().BeEquivalentTo(GrantTypes.ResourceOwnerPassword);
        }
    }
}
