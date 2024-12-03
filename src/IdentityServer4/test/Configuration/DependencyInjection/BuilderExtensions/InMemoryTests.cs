using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration.DependencyInjection.BuilderExtensions
{
    public class InMemoryTests
    {
        private IIdentityServerBuilder _builder;
        private IServiceCollection _services;

        public InMemoryTests()
        {
            _services = new ServiceCollection();
            _builder = new IdentityServerBuilder(_services);
        }

        [Fact]
        public void AddInMemoryCaching_RegistersExpectedServices()
        {
            _builder.AddInMemoryCaching();

            _services.Should().Contain(x => x.ServiceType == typeof(IMemoryCache));
            _services.Should().Contain(x => x.ServiceType == typeof(ICache<>));
        }

        [Fact]
        public void AddInMemoryIdentityResources_RegistersExpectedServices()
        {
            var identityResources = new[] { new IdentityResource() };
            _builder.AddInMemoryIdentityResources(identityResources);

            _services.Should().Contain(x => x.ServiceType == typeof(IEnumerable<IdentityResource>));
            _services.Should().Contain(x => x.ServiceType == typeof(IResourceStore));
        }

        [Fact]
        public void AddInMemoryApiResources_RegistersExpectedServices()
        {
            var apiResources = new[] { new ApiResource("test") };
            _builder.AddInMemoryApiResources(apiResources);

            _services.Should().Contain(x => x.ServiceType == typeof(IEnumerable<ApiResource>));
            _services.Should().Contain(x => x.ServiceType == typeof(IResourceStore));
        }

        [Fact]
        public void AddInMemoryApiScopes_RegistersExpectedServices()
        {
            var apiScopes = new[] { new ApiScope("test") };
            _builder.AddInMemoryApiScopes(apiScopes);

            _services.Should().Contain(x => x.ServiceType == typeof(IEnumerable<ApiScope>));
            _services.Should().Contain(x => x.ServiceType == typeof(IResourceStore));
        }

        [Fact]
        public void AddInMemoryClients_RegistersExpectedServices()
        {
            var clients = new[] { new Client { ClientId = "test" } };
            _builder.AddInMemoryClients(clients);

            _services.Should().Contain(x => x.ServiceType == typeof(IEnumerable<Client>));
            _services.Should().Contain(x => x.ServiceType == typeof(IClientStore));
            _services.Should().Contain(x => x.ServiceType == typeof(ICorsPolicyService));
        }

        [Fact]
        public void AddInMemoryPersistedGrants_RegistersExpectedServices()
        {
            _builder.AddInMemoryPersistedGrants();

            _services.Should().Contain(x => x.ServiceType == typeof(IPersistedGrantStore));
            _services.Should().Contain(x => x.ServiceType == typeof(IDeviceFlowStore));
        }

        [Fact]
        public void AddInMemoryIdentityResources_WithConfigurationSection_RegistersExpectedServices()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["0:Name"] = "openid",
                    ["0:DisplayName"] = "OpenID",
                    ["1:Name"] = "profile",
                    ["1:DisplayName"] = "Profile"
                })
                .Build();

            _builder.AddInMemoryIdentityResources(config);

            var resources = _services
                .SingleOrDefault(x => x.ServiceType == typeof(IEnumerable<IdentityResource>))
                ?.ImplementationInstance as IEnumerable<IdentityResource>;

            resources.Should().HaveCount(2);
            resources.Should().Contain(x => x.Name == "openid");
            resources.Should().Contain(x => x.Name == "profile");
        }
    }
}
