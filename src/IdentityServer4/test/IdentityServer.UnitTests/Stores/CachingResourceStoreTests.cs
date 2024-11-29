using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Xunit;
using IdentityServer4.Configuration;
using NSubstitute;

namespace IdentityServer.UnitTests.Stores
{
    public class CachingResourceStoreTests
    {
        private readonly IResourceStore _inner;
        private readonly ICache<IEnumerable<IdentityResource>> _identityCache;
        private readonly ICache<IEnumerable<ApiResource>> _apiCache;
        private readonly ICache<IEnumerable<ApiScope>> _scopeCache;
        private readonly ICache<IEnumerable<ApiResource>> _apiByScopeCache;
        private readonly ICache<Resources> _allCache;
        private readonly CachingResourceStore<IResourceStore> _subject;
        private readonly IdentityServerOptions _options;

        public CachingResourceStoreTests()
        {
            _inner = Substitute.For<IResourceStore>();
            _identityCache = Substitute.For<ICache<IEnumerable<IdentityResource>>>();
            _apiCache = Substitute.For<ICache<IEnumerable<ApiResource>>>();
            _scopeCache = Substitute.For<ICache<IEnumerable<ApiScope>>>();
            _apiByScopeCache = Substitute.For<ICache<IEnumerable<ApiResource>>>();
            _allCache = Substitute.For<ICache<Resources>>();
            _options = new IdentityServerOptions();

            _subject = new CachingResourceStore<IResourceStore>(
                _options,
                _inner,
                _identityCache,
                _apiByScopeCache,
                _apiCache,
                _scopeCache,
                _allCache,
                Substitute.For<ILogger<CachingResourceStore<IResourceStore>>>()
            );
        }

        [Fact]
        public async Task GetAllResources_should_cache_result()
        {
            // Arrange
            var resources = new Resources();
            _inner.GetAllResourcesAsync().Returns(resources);

            // Act
            var result = await _subject.GetAllResourcesAsync();

            // Assert
            result.Should().BeSameAs(resources);
            await _allCache.Received(1).GetAsync("__all__", 
                _options.Caching.ResourceStoreExpiration,
                Arg.Any<Func<Task<Resources>>>());
        }

        [Fact]
        public async Task FindApiResourcesByName_should_cache_result()
        {
            // Arrange
            var names = new[] { "api1", "api2" };
            var resources = new[] { new ApiResource("api1"), new ApiResource("api2") };
            _inner.FindApiResourcesByNameAsync(names).Returns(resources);

            // Act
            var result = await _subject.FindApiResourcesByNameAsync(names);

            // Assert
            result.Should().BeEquivalentTo(resources);
            await _apiCache.Received(1).GetAsync("api1,api2",
                _options.Caching.ResourceStoreExpiration,
                Arg.Any<Func<Task<IEnumerable<ApiResource>>>>());
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeName_should_cache_result()
        {
            // Arrange
            var names = new[] { "id1", "id2" };
            var resources = new[] { new IdentityResource("id1"), new IdentityResource("id2") };
            _inner.FindIdentityResourcesByScopeNameAsync(names).Returns(resources);

            // Act
            var result = await _subject.FindIdentityResourcesByScopeNameAsync(names);

            // Assert
            result.Should().BeEquivalentTo(resources);
            await _identityCache.Received(1).GetAsync("id1,id2",
                _options.Caching.ResourceStoreExpiration,
                Arg.Any<Func<Task<IEnumerable<IdentityResource>>>>());
        }

        [Fact]
        public async Task FindApiScopesByName_should_cache_result()
        {
            // Arrange
            var names = new[] { "scope1", "scope2" };
            var scopes = new[] { new ApiScope("scope1"), new ApiScope("scope2") };
            _inner.FindApiScopesByNameAsync(names).Returns(scopes);

            // Act
            var result = await _subject.FindApiScopesByNameAsync(names);

            // Assert
            result.Should().BeEquivalentTo(scopes);
            await _scopeCache.Received(1).GetAsync("scope1,scope2",
                _options.Caching.ResourceStoreExpiration,
                Arg.Any<Func<Task<IEnumerable<ApiScope>>>>());
        }

        [Fact]
        public async Task FindApiResourcesByScopeName_should_cache_result()
        {
            // Arrange
            var names = new[] { "scope1", "scope2" };
            var resources = new[] { new ApiResource("api1"), new ApiResource("api2") };
            _inner.FindApiResourcesByScopeNameAsync(names).Returns(resources);

            // Act
            var result = await _subject.FindApiResourcesByScopeNameAsync(names);

            // Assert
            result.Should().BeEquivalentTo(resources);
            await _apiByScopeCache.Received(1).GetAsync("scope1,scope2",
                _options.Caching.ResourceStoreExpiration,
                Arg.Any<Func<Task<IEnumerable<ApiResource>>>>());
        }
    }
}
