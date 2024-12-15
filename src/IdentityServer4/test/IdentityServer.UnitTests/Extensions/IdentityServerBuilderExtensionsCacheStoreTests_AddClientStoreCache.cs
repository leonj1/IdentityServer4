using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class IdentityServerBuilderExtensionsCacheStoreTests_AddClientStoreCache
    {
        private class CustomClientStore : IClientStore
        {
            public Task<Client> FindClientByIdAsync(string clientId)
            {
                throw new System.NotImplementedException();
            }
        }

        [Fact]
        public void AddClientStoreCache_should_add_concrete_iclientstore_implementation()
        {
            var services = new ServiceCollection();
            var identityServerBuilder = new IdentityServerBuilder(services);

            identityServerBuilder.AddClientStoreCache<CustomClientStore>();

            services.Any(x => x.ImplementationType == typeof(CustomClientStore)).Should().BeTrue();
        }
    }
}
