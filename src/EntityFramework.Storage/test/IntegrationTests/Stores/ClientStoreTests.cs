using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Sdk;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ClientStoreTests : IntegrationTest<ClientStoreTests, ConfigurationDbContext, ConfigurationStoreOptions>
    {
        public ClientStoreTests(DatabaseProviderFixture<ConfigurationDbContext> fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindClientByIdAsync_WithNoClients_ShouldReturnNull()
        {
            using (var context = CreateContext())
            {
                var store = new ClientStore(context, FakeLogger<ClientStore>());
                var client = await store.FindClientByIdAsync("nonexistent");
                client.Should().BeNull();
            }
        }

        [Fact]
        public async Task FindClientByIdAsync_WithSingleClient_ShouldReturnClient()
        {
            using (var context = CreateContext())
            {
                var testClient = new Client
                {
                    ClientId = "test_client",
                    ClientName = "Test client"
                };

                context.Clients.Add(testClient.ToEntity());
                await context.SaveChangesAsync();

                var store = new ClientStore(context, FakeLogger<ClientStore>());
                var client = await store.FindClientByIdAsync("test_client");
                client.Should().BeEquivalentTo(testClient);
            }
        }

        [Fact]
        public async Task FindClientByIdAsync_WithMultipleClients_ShouldReturnCorrectClient()
        {
            using (var context = CreateContext())
            {
                var testClient1 = new Client
                {
                    ClientId = "test_client1",
                    ClientName = "Test client 1"
                };

                var testClient2 = new Client
                {
                    ClientId = "test_client2",
                    ClientName = "Test client 2"
                };

                context.Clients.Add(testClient1.ToEntity());
                context.Clients.Add(testClient2.ToEntity());
                await context.SaveChangesAsync();

                var store = new ClientStore(context, FakeLogger<ClientStore>());
                var client = await store.FindClientByIdAsync("test_client2");
                client.Should().BeEquivalentTo(testClient2);
            }
        }

        [Fact]
        public async Task FindClientByIdAsync_WithLargeNumberOfClients_ShouldReturnCorrectClient()
        {
            using (var context = CreateContext())
            {
                var testClient = new Client
                {
                    ClientId = "test_client",
                    ClientName = "Test client"
                };

                for (int i = 0; i < 100; i++)
                {
                    context.Clients.Add(new Client
                    {
                        ClientId = $"test_client_{i}",
                        ClientName = $"Test client {i}"
                    }.ToEntity());
                }

                context.Clients.Add(testClient.ToEntity());
                await context.SaveChangesAsync();

                var store = new ClientStore(context, FakeLogger<ClientStore>());
                var client = await store.FindClientByIdAsync("test_client");
                client.Should().BeEquivalentTo(testClient);
            }
        }
    }
}
