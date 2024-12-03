using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Services
{
    public class CachingCorsPolicyServiceTests
    {
        private readonly ILogger<CachingCorsPolicyService<MockCorsPolicyService>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly MockCorsPolicyService _mockInnerService;
        private readonly ICache<CachingCorsPolicyService<MockCorsPolicyService>.CorsCacheEntry> _cache;
        private readonly CachingCorsPolicyService<MockCorsPolicyService> _subject;

        public CachingCorsPolicyServiceTests()
        {
            _logger = new LoggerFactory().CreateLogger<CachingCorsPolicyService<MockCorsPolicyService>>();
            _options = new IdentityServerOptions();
            _mockInnerService = new MockCorsPolicyService();
            _cache = new DefaultCache<CachingCorsPolicyService<MockCorsPolicyService>.CorsCacheEntry>();
            _subject = new CachingCorsPolicyService<MockCorsPolicyService>(
                _options,
                _mockInnerService,
                _cache,
                _logger
            );
        }

        [Fact]
        public async Task IsOriginAllowed_WhenOriginAllowed_ShouldReturnTrueAndCache()
        {
            // Arrange
            var origin = "https://allowed-origin.com";
            _mockInnerService.Response = true;

            // Act
            var result1 = await _subject.IsOriginAllowedAsync(origin);
            var result2 = await _subject.IsOriginAllowedAsync(origin);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            _mockInnerService.TimesCalled.Should().Be(1); // Verify caching works
        }

        [Fact]
        public async Task IsOriginAllowed_WhenOriginNotAllowed_ShouldReturnFalseAndCache()
        {
            // Arrange
            var origin = "https://not-allowed-origin.com";
            _mockInnerService.Response = false;

            // Act
            var result1 = await _subject.IsOriginAllowedAsync(origin);
            var result2 = await _subject.IsOriginAllowedAsync(origin);

            // Assert
            result1.Should().BeFalse();
            result2.Should().BeFalse();
            _mockInnerService.TimesCalled.Should().Be(1); // Verify caching works
        }

        private class MockCorsPolicyService : ICorsPolicyService
        {
            public bool Response { get; set; }
            public int TimesCalled { get; private set; }

            public Task<bool> IsOriginAllowedAsync(string origin)
            {
                TimesCalled++;
                return Task.FromResult(Response);
            }
        }

        private class DefaultCache<T> : ICache<T>
        {
            private T _item;

            public Task<T> GetAsync(string key)
            {
                return Task.FromResult(_item);
            }

            public Task SetAsync(string key, T item, TimeSpan expiration)
            {
                _item = item;
                return Task.CompletedTask;
            }
        }
    }
}
