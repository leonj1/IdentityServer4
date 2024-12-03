using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Xunit;
using FluentAssertions;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class IResourceStoreTests
    {
        private readonly Mock<IResourceStore> _mockResourceStore;

        public IResourceStoreTests()
        {
            _mockResourceStore = new Mock<IResourceStore>();
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_ShouldReturnMatchingResources()
        {
            // Arrange
            var scopeNames = new[] { "openid", "profile" };
            var expectedResources = new List<IdentityResource>
            {
                new IdentityResource("openid", "OpenID", new[] { "sub" }),
                new IdentityResource("profile", "Profile", new[] { "name", "email" })
            };

            _mockResourceStore.Setup(x => x.FindIdentityResourcesByScopeNameAsync(scopeNames))
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _mockResourceStore.Object.FindIdentityResourcesByScopeNameAsync(scopeNames);

            // Assert
            result.Should().BeEquivalentTo(expectedResources);
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_ShouldReturnMatchingScopes()
        {
            // Arrange
            var scopeNames = new[] { "api1", "api2" };
            var expectedScopes = new List<ApiScope>
            {
                new ApiScope("api1", "API 1"),
                new ApiScope("api2", "API 2")
            };

            _mockResourceStore.Setup(x => x.FindApiScopesByNameAsync(scopeNames))
                .ReturnsAsync(expectedScopes);

            // Act
            var result = await _mockResourceStore.Object.FindApiScopesByNameAsync(scopeNames);

            // Assert
            result.Should().BeEquivalentTo(expectedScopes);
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_ShouldReturnMatchingResources()
        {
            // Arrange
            var scopeNames = new[] { "api1", "api2" };
            var expectedResources = new List<ApiResource>
            {
                new ApiResource("api1", "API 1"),
                new ApiResource("api2", "API 2")
            };

            _mockResourceStore.Setup(x => x.FindApiResourcesByScopeNameAsync(scopeNames))
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _mockResourceStore.Object.FindApiResourcesByScopeNameAsync(scopeNames);

            // Assert
            result.Should().BeEquivalentTo(expectedResources);
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_ShouldReturnMatchingResources()
        {
            // Arrange
            var resourceNames = new[] { "api1", "api2" };
            var expectedResources = new List<ApiResource>
            {
                new ApiResource("api1", "API 1"),
                new ApiResource("api2", "API 2")
            };

            _mockResourceStore.Setup(x => x.FindApiResourcesByNameAsync(resourceNames))
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _mockResourceStore.Object.FindApiResourcesByNameAsync(resourceNames);

            // Assert
            result.Should().BeEquivalentTo(expectedResources);
        }

        [Fact]
        public async Task GetAllResourcesAsync_ShouldReturnAllResources()
        {
            // Arrange
            var expectedResources = new Resources(
                new[] { new IdentityResource("openid", "OpenID") },
                new[] { new ApiResource("api1", "API 1") },
                new[] { new ApiScope("scope1", "Scope 1") }
            );

            _mockResourceStore.Setup(x => x.GetAllResourcesAsync())
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _mockResourceStore.Object.GetAllResourcesAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedResources);
        }
    }
}
