using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemoryResourcesStoreTests
    {
        [Fact]
        public void InMemoryResourcesStore_should_throw_if_contains_duplicate_names()
        {
            List<IdentityResource> identityResources = new List<IdentityResource>
            {
                new IdentityResource { Name = "A" },
                new IdentityResource { Name = "A" },
                new IdentityResource { Name = "C" }
            };

            List<ApiResource> apiResources = new List<ApiResource>
            {
                new ApiResource { Name = "B" },
                new ApiResource { Name = "B" },
                new ApiResource { Name = "C" }
            };

            List<ApiScope> scopes = new List<ApiScope>
            {
                new ApiScope { Name = "B" },
                new ApiScope { Name = "C" },
                new ApiScope { Name = "C" },
            };

            Action act = () => new InMemoryResourcesStore(identityResources, null, null);
            act.Should().Throw<ArgumentException>();

            act = () => new InMemoryResourcesStore(null, apiResources, null);
            act.Should().Throw<ArgumentException>();
            
            act = () => new InMemoryResourcesStore(null, null, scopes);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void InMemoryResourcesStore_should_not_throw_if_does_not_contains_duplicate_names()
        {
            List<IdentityResource> identityResources = new List<IdentityResource>
            {
                new IdentityResource { Name = "A" },
                new IdentityResource { Name = "B" },
                new IdentityResource { Name = "C" }
            };

            List<ApiResource> apiResources = new List<ApiResource>
            {
                new ApiResource { Name = "A" },
                new ApiResource { Name = "B" },
                new ApiResource { Name = "C" }
            };

            List<ApiScope> apiScopes = new List<ApiScope>
            {
                new ApiScope { Name = "A" },
                new ApiScope { Name = "B" },
                new ApiScope { Name = "C" },
            };
            
            new InMemoryResourcesStore(identityResources, null, null);
            new InMemoryResourcesStore(null, apiResources, null);
            new InMemoryResourcesStore(null, null, apiScopes);
        }
        [Fact]
        public async Task GetAllResourcesAsync_WhenResourcesExist_ShouldReturnAll()
        {
            // Arrange
            var identityResources = new List<IdentityResource> { new IdentityResource { Name = "id1" } };
            var apiResources = new List<ApiResource> { new ApiResource { Name = "api1" } };
            var apiScopes = new List<ApiScope> { new ApiScope { Name = "scope1" } };
            var store = new InMemoryResourcesStore(identityResources, apiResources, apiScopes);

            // Act
            var resources = await store.GetAllResourcesAsync();

            // Assert
            resources.IdentityResources.Should().BeEquivalentTo(identityResources);
            resources.ApiResources.Should().BeEquivalentTo(apiResources);
            resources.ApiScopes.Should().BeEquivalentTo(apiScopes);
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceExists_ShouldReturnMatch()
        {
            // Arrange
            var apiResources = new List<ApiResource> 
            { 
                new ApiResource { Name = "api1" },
                new ApiResource { Name = "api2" }
            };
            var store = new InMemoryResourcesStore(apiResources: apiResources);

            // Act
            var resources = await store.FindApiResourcesByNameAsync(new[] { "api1" });

            // Assert
            resources.Should().ContainSingle(r => r.Name == "api1");
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourceExists_ShouldReturnMatch()
        {
            // Arrange
            var identityResources = new List<IdentityResource> 
            { 
                new IdentityResource { Name = "id1" },
                new IdentityResource { Name = "id2" }
            };
            var store = new InMemoryResourcesStore(identityResources: identityResources);

            // Act
            var resources = await store.FindIdentityResourcesByScopeNameAsync(new[] { "id1" });

            // Assert
            resources.Should().ContainSingle(r => r.Name == "id1");
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourceExists_ShouldReturnMatch()
        {
            // Arrange
            var apiResources = new List<ApiResource> 
            { 
                new ApiResource { Name = "api1", Scopes = { "scope1" } },
                new ApiResource { Name = "api2", Scopes = { "scope2" } }
            };
            var store = new InMemoryResourcesStore(apiResources: apiResources);

            // Act
            var resources = await store.FindApiResourcesByScopeNameAsync(new[] { "scope1" });

            // Assert
            resources.Should().ContainSingle(r => r.Name == "api1");
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenScopeExists_ShouldReturnMatch()
        {
            // Arrange
            var apiScopes = new List<ApiScope> 
            { 
                new ApiScope { Name = "scope1" },
                new ApiScope { Name = "scope2" }
            };
            var store = new InMemoryResourcesStore(apiScopes: apiScopes);

            // Act
            var scopes = await store.FindApiScopesByNameAsync(new[] { "scope1" });

            // Assert
            scopes.Should().ContainSingle(s => s.Name == "scope1");
        }

        [Fact]
        public async Task GetAllResourcesAsync_WhenNoResourcesExist_ShouldReturnEmpty()
        {
            // Arrange
            var store = new InMemoryResourcesStore(null, null, null);

            // Act
            var resources = await store.GetAllResourcesAsync();

            // Assert
            resources.IdentityResources.Should().BeEmpty();
            resources.ApiResources.Should().BeEmpty();
            resources.ApiScopes.Should().BeEmpty();
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceDoesNotExist_ShouldReturnEmpty()
        {
            // Arrange
            var apiResources = new List<ApiResource> { new ApiResource { Name = "api1" } };
            var store = new InMemoryResourcesStore(apiResources: apiResources);

            // Act
            var resources = await store.FindApiResourcesByNameAsync(new[] { "nonexistent" });

            // Assert
            resources.Should().BeEmpty();
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenNullNamesPassed_ShouldReturnEmpty()
        {
            // Arrange
            var apiResources = new List<ApiResource> { new ApiResource { Name = "api1" } };
            var store = new InMemoryResourcesStore(apiResources: apiResources);

            // Act
            var resources = await store.FindApiResourcesByNameAsync(null);

            // Assert
            resources.Should().BeEmpty();
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenNullScopesPassed_ShouldReturnEmpty()
        {
            // Arrange
            var identityResources = new List<IdentityResource> { new IdentityResource { Name = "id1" } };
            var store = new InMemoryResourcesStore(identityResources: identityResources);

            // Act
            var resources = await store.FindIdentityResourcesByScopeNameAsync(null);

            // Assert
            resources.Should().BeEmpty();
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenNullNamesPassed_ShouldReturnEmpty()
        {
            // Arrange
            var apiScopes = new List<ApiScope> { new ApiScope { Name = "scope1" } };
            var store = new InMemoryResourcesStore(apiScopes: apiScopes);

            // Act
            var scopes = await store.FindApiScopesByNameAsync(null);

            // Assert
            scopes.Should().BeEmpty();
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenMultipleResourcesShareScope_ShouldReturnAll()
        {
            // Arrange
            var apiResources = new List<ApiResource> 
            { 
                new ApiResource { Name = "api1", Scopes = { "shared" } },
                new ApiResource { Name = "api2", Scopes = { "shared" } }
            };
            var store = new InMemoryResourcesStore(apiResources: apiResources);

            // Act
            var resources = await store.FindApiResourcesByScopeNameAsync(new[] { "shared" });

            // Assert
            resources.Should().HaveCount(2);
            resources.Should().Contain(r => r.Name == "api1");
            resources.Should().Contain(r => r.Name == "api2");
        }
    }
}
