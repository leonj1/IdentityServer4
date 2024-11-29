using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemoryClientStoreTests
    {
        [Fact]
        public void InMemoryClient_should_throw_if_contain_duplicate_client_ids()
        {
            List<Client> clients = new List<Client>
            {
                new Client { ClientId = "1"},
                new Client { ClientId = "1"},
                new Client { ClientId = "3"}
            };

            Action act = () => new InMemoryClientStore(clients);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void InMemoryClient_should_not_throw_if_does_not_contain_duplicate_client_ids()
        {
            List<Client> clients = new List<Client>
            {
                new Client { ClientId = "1"},
                new Client { ClientId = "2"},
                new Client { ClientId = "3"}
            };

            new InMemoryClientStore(clients);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ReturnsClient()
        {
            // Arrange
            var expectedClient = new Client { ClientId = "test_client" };
            var clients = new[] { expectedClient };
            var store = new InMemoryClientStore(clients);

            // Act
            var client = await store.FindClientByIdAsync("test_client");

            // Assert
            client.Should().NotBeNull();
            client.Should().BeSameAs(expectedClient);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ReturnsNull()
        {
            // Arrange
            var clients = new[] { new Client { ClientId = "test_client" } };
            var store = new InMemoryClientStore(clients);

            // Act
            var client = await store.FindClientByIdAsync("non_existent_client");

            // Assert
            client.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenClientsIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action act = () => new InMemoryClientStore(null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("clients");
        }

        [Fact]
        public async Task FindClientByIdAsync_WithEmptyClientList_ReturnsNull()
        {
            // Arrange
            var store = new InMemoryClientStore(new Client[] { });

            // Act
            var client = await store.FindClientByIdAsync("any_client");

            // Assert
            client.Should().BeNull();
        }

        [Fact]
        public async Task FindClientByIdAsync_IsCaseSensitive()
        {
            // Arrange
            var expectedClient = new Client { ClientId = "Test_Client" };
            var store = new InMemoryClientStore(new[] { expectedClient });

            // Act
            var client1 = await store.FindClientByIdAsync("Test_Client");
            var client2 = await store.FindClientByIdAsync("test_client");

            // Assert
            client1.Should().NotBeNull();
            client1.Should().BeSameAs(expectedClient);
            client2.Should().BeNull();
        }
    }
}
