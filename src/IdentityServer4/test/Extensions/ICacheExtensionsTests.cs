using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class ICacheExtensionsTests
    {
        private readonly Mock<ICache<string>> _cache;
        private readonly Mock<ILogger> _logger;
        private readonly TimeSpan _duration;

        public ICacheExtensionsTests()
        {
            _cache = new Mock<ICache<string>>();
            _logger = new Mock<ILogger>();
            _duration = TimeSpan.FromMinutes(5);
        }

        [Fact]
        public async Task GetAsync_WhenKeyIsNull_ReturnsNull()
        {
            // Act
            var result = await _cache.Object.GetAsync<string>(null, _duration, () => Task.FromResult("test"), _logger.Object);

            // Assert
            result.Should().BeNull();
            _cache.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetAsync_WhenCacheHit_ReturnsFromCache()
        {
            // Arrange
            var cacheKey = "test-key";
            var cachedValue = "cached-value";
            _cache.Setup(x => x.GetAsync(cacheKey)).ReturnsAsync(cachedValue);

            // Act
            var result = await _cache.Object.GetAsync(cacheKey, _duration, 
                () => Task.FromResult("new-value"), _logger.Object);

            // Assert
            result.Should().Be(cachedValue);
            _cache.Verify(x => x.GetAsync(cacheKey), Times.Once);
            _cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task GetAsync_WhenCacheMiss_SetsAndReturnsNewValue()
        {
            // Arrange
            var cacheKey = "test-key";
            var newValue = "new-value";
            _cache.Setup(x => x.GetAsync(cacheKey)).ReturnsAsync((string)null);

            // Act
            var result = await _cache.Object.GetAsync(cacheKey, _duration,
                () => Task.FromResult(newValue), _logger.Object);

            // Assert
            result.Should().Be(newValue);
            _cache.Verify(x => x.GetAsync(cacheKey), Times.Once);
            _cache.Verify(x => x.SetAsync(cacheKey, newValue, _duration), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WhenGetFunctionReturnsNull_DoesNotCache()
        {
            // Arrange
            var cacheKey = "test-key";
            _cache.Setup(x => x.GetAsync(cacheKey)).ReturnsAsync((string)null);

            // Act
            var result = await _cache.Object.GetAsync(cacheKey, _duration,
                () => Task.FromResult<string>(null), _logger.Object);

            // Assert
            result.Should().BeNull();
            _cache.Verify(x => x.GetAsync(cacheKey), Times.Once);
            _cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task GetAsync_WhenCacheIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                ICacheExtensions.GetAsync<string>(null, "key", _duration, () => Task.FromResult("test"), _logger.Object));
        }

        [Fact]
        public async Task GetAsync_WhenGetFunctionIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _cache.Object.GetAsync<string>("key", _duration, null, _logger.Object));
        }
    }
}
