using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class IClientStoreExtensionsTests
    {
        private class TestClientStore : IClientStore
        {
            private readonly Client _client;
            
            public TestClientStore(Client client)
            {
                _client = client;
            }

            public Task<Client> FindClientByIdAsync(string clientId)
            {
                if (clientId == "test_client")
                    return Task.FromResult(_client);
                return Task.FromResult<Client>(null);
            }
        }

        [Fact]
        public async Task FindEnabledClientByIdAsync_WhenClientExistsAndEnabled_ShouldReturnClient()
        {
            // Arrange
            var expectedClient = new Client
            {
                ClientId = "test_client",
                Enabled = true
            };
            var store = new TestClientStore(expectedClient);

            // Act
            var client = await store.FindEnabledClientByIdAsync("test_client");

            // Assert
            client.Should().NotBeNull();
            client.Should().BeSameAs(expectedClient);
        }

        [Fact]
        public async Task FindEnabledClientByIdAsync_WhenClientExistsButDisabled_ShouldReturnNull()
        {
            // Arrange
            var disabledClient = new Client
            {
                ClientId = "test_client",
                Enabled = false
            };
            var store = new TestClientStore(disabledClient);

            // Act
            var client = await store.FindEnabledClientByIdAsync("test_client");

            // Assert
            client.Should().BeNull();
        }

        [Fact]
        public async Task FindEnabledClientByIdAsync_WhenClientDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var store = new TestClientStore(null);

            // Act
            var client = await store.FindEnabledClientByIdAsync("non_existent_client");

            // Assert
            client.Should().BeNull();
        }
    }
}
