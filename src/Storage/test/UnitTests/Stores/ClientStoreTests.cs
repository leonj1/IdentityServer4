using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.UnitTests.Stores
{
    public class ClientStoreTests
    {
        private class TestClientStore : IClientStore
        {
            private readonly Client _client;

            public TestClientStore(Client client = null)
            {
                _client = client;
            }

            public Task<Client> FindClientByIdAsync(string clientId)
            {
                if (_client?.ClientId == clientId)
                {
                    return Task.FromResult(_client);
                }
                return Task.FromResult<Client>(null);
            }
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ShouldReturnClient()
        {
            // Arrange
            var expectedClient = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };
            var clientStore = new TestClientStore(expectedClient);

            // Act
            var result = await clientStore.FindClientByIdAsync("test_client");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedClient.ClientId, result.ClientId);
            Assert.Equal(expectedClient.ClientName, result.ClientName);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var clientStore = new TestClientStore();

            // Act
            var result = await clientStore.FindClientByIdAsync("non_existent_client");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientIdDoesNotMatch_ShouldReturnNull()
        {
            // Arrange
            var existingClient = new Client
            {
                ClientId = "existing_client",
                ClientName = "Existing Client"
            };
            var clientStore = new TestClientStore(existingClient);

            // Act
            var result = await clientStore.FindClientByIdAsync("different_client");

            // Assert
            Assert.Null(result);
        }
    }
}
