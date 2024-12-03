using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class ReplayCacheTests
    {
        private readonly Mock<IReplayCache> _mockReplayCache;

        public ReplayCacheTests()
        {
            _mockReplayCache = new Mock<IReplayCache>();
        }

        [Fact]
        public async Task AddAsync_ShouldStoreHandle()
        {
            // Arrange
            var purpose = "test-purpose";
            var handle = "test-handle";
            var expiration = DateTimeOffset.UtcNow.AddMinutes(5);

            _mockReplayCache.Setup(x => x.AddAsync(purpose, handle, expiration))
                .Returns(Task.CompletedTask);

            // Act
            await _mockReplayCache.Object.AddAsync(purpose, handle, expiration);

            // Assert
            _mockReplayCache.Verify(x => x.AddAsync(purpose, handle, expiration), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_WhenHandleExists_ShouldReturnTrue()
        {
            // Arrange
            var purpose = "test-purpose";
            var handle = "test-handle";

            _mockReplayCache.Setup(x => x.ExistsAsync(purpose, handle))
                .ReturnsAsync(true);

            // Act
            var result = await _mockReplayCache.Object.ExistsAsync(purpose, handle);

            // Assert
            Assert.True(result);
            _mockReplayCache.Verify(x => x.ExistsAsync(purpose, handle), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_WhenHandleDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var purpose = "test-purpose";
            var handle = "test-handle";

            _mockReplayCache.Setup(x => x.ExistsAsync(purpose, handle))
                .ReturnsAsync(false);

            // Act
            var result = await _mockReplayCache.Object.ExistsAsync(purpose, handle);

            // Assert
            Assert.False(result);
            _mockReplayCache.Verify(x => x.ExistsAsync(purpose, handle), Times.Once);
        }
    }
}
