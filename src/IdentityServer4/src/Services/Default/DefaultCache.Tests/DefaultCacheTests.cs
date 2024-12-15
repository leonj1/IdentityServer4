using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace IdentityServer4.UnitTests.Services
{
    public class DefaultCacheTests
    {
        private readonly IMemoryCache _cache;
        private readonly DefaultCache<string> _subject;

        public DefaultCacheTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _subject = new DefaultCache<string>(_cache);
        }

        [Fact]
        public async Task GetAsync_WhenKeyExists_ShouldReturnCachedItem()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = "testValue";
            await _subject.SetAsync(key, expectedValue, TimeSpan.FromMinutes(1));

            // Act
            var result = await _subject.GetAsync(key);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _subject.GetAsync("nonexistentKey");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_ShouldCacheItemWithExpiration()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";
            var expiration = TimeSpan.FromMilliseconds(50);

            // Act
            await _subject.SetAsync(key, value, expiration);
            var initialResult = await _subject.GetAsync(key);
            await Task.Delay(100); // Wait for expiration
            var afterExpirationResult = await _subject.GetAsync(key);

            // Assert
            Assert.Equal(value, initialResult);
            Assert.Null(afterExpirationResult);
        }

        [Fact]
        public async Task GetKey_ShouldIncludeTypeNameInKey()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";
            await _subject.SetAsync(key, value, TimeSpan.FromMinutes(1));

            // Act & Assert
            // Verify the item is stored with the type name prefix
            var result = await _subject.GetAsync(key);
            Assert.Equal(value, result);

            // Try to get the same key with a different type
            var differentCache = new DefaultCache<int>(_cache);
            var differentResult = await differentCache.GetAsync(key);
            Assert.Null(differentResult);
        }
    }
}
