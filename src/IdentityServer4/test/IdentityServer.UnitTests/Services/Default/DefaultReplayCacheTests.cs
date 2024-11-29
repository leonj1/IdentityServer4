using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultReplayCacheTests
    {
        private readonly TestCache _cache = new TestCache();
        private readonly DefaultReplayCache _subject;

        public DefaultReplayCacheTests()
        {
            _subject = new DefaultReplayCache(_cache);
        }

        [Fact]
        public async Task AddAsync_WhenCalled_ShouldAddToCache()
        {
            // Arrange
            var purpose = "purpose";
            var handle = "handle";
            var expiration = DateTimeOffset.UtcNow.AddMinutes(5);

            // Act
            await _subject.AddAsync(purpose, handle, expiration);

            // Assert
            var key = "DefaultReplayCache:purposehandle";
            _cache.Items.ContainsKey(key).Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WhenItemExists_ShouldReturnTrue()
        {
            // Arrange
            var purpose = "purpose";
            var handle = "handle";
            var expiration = DateTimeOffset.UtcNow.AddMinutes(5);
            await _subject.AddAsync(purpose, handle, expiration);

            // Act
            var result = await _subject.ExistsAsync(purpose, handle);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WhenItemDoesNotExist_ShouldReturnFalse()
        {
            // Act
            var result = await _subject.ExistsAsync("purpose", "handle");

            // Assert
            result.Should().BeFalse();
        }

        private class TestCache : IDistributedCache
        {
            public Dictionary<string, byte[]> Items { get; } = new Dictionary<string, byte[]>();

            public byte[] Get(string key) => Items.ContainsKey(key) ? Items[key] : null;
            public Task<byte[]> GetAsync(string key, CancellationToken token = default) => 
                Task.FromResult(Get(key));

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => 
                Items[key] = value;
            public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
            {
                Set(key, value, options);
                return Task.CompletedTask;
            }

            public void Refresh(string key) => throw new NotImplementedException();
            public Task RefreshAsync(string key, CancellationToken token = default) => throw new NotImplementedException();
            public void Remove(string key) => Items.Remove(key);
            public Task RemoveAsync(string key, CancellationToken token = default)
            {
                Remove(key);
                return Task.CompletedTask;
            }
        }
    }
}
