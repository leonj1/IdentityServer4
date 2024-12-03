using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Stores
{
    public class DefaultRefreshTokenStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _mockPersistedGrantStore;
        private readonly Mock<IPersistentGrantSerializer> _mockSerializer;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerationService;
        private readonly Mock<ILogger<DefaultRefreshTokenStore>> _mockLogger;
        private readonly DefaultRefreshTokenStore _store;

        public DefaultRefreshTokenStoreTests()
        {
            _mockPersistedGrantStore = new Mock<IPersistedGrantStore>();
            _mockSerializer = new Mock<IPersistentGrantSerializer>();
            _mockHandleGenerationService = new Mock<IHandleGenerationService>();
            _mockLogger = new Mock<ILogger<DefaultRefreshTokenStore>>();

            _store = new DefaultRefreshTokenStore(
                _mockPersistedGrantStore.Object,
                _mockSerializer.Object,
                _mockHandleGenerationService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task StoreRefreshTokenAsync_ShouldStoreToken()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600
            };
            var handle = "handle123";

            _mockHandleGenerationService
                .Setup(x => x.GenerateAsync())
                .ReturnsAsync(handle);

            // Act
            var result = await _store.StoreRefreshTokenAsync(refreshToken);

            // Assert
            result.Should().Be(handle);
            _mockPersistedGrantStore.Verify(x => x.StoreAsync(It.IsAny<PersistedGrant>()), Times.Once);
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var handle = "handle123";
            var refreshToken = new RefreshToken
            {
                ClientId = "client1",
                SubjectId = "subject1"
            };
            var serializedRefreshToken = "serialized_token";

            _mockPersistedGrantStore
                .Setup(x => x.GetAsync(handle))
                .ReturnsAsync(new PersistedGrant { Data = serializedRefreshToken });

            _mockSerializer
                .Setup(x => x.Deserialize<RefreshToken>(serializedRefreshToken))
                .Returns(refreshToken);

            // Act
            var result = await _store.GetRefreshTokenAsync(handle);

            // Assert
            result.Should().BeEquivalentTo(refreshToken);
        }

        [Fact]
        public async Task RemoveRefreshTokenAsync_ShouldRemoveToken()
        {
            // Arrange
            var handle = "handle123";

            // Act
            await _store.RemoveRefreshTokenAsync(handle);

            // Assert
            _mockPersistedGrantStore.Verify(x => x.RemoveAsync(handle), Times.Once);
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldUpdateToken()
        {
            // Arrange
            var handle = "handle123";
            var refreshToken = new RefreshToken
            {
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600
            };

            // Act
            await _store.UpdateRefreshTokenAsync(handle, refreshToken);

            // Assert
            _mockPersistedGrantStore.Verify(x => x.StoreAsync(It.IsAny<PersistedGrant>()), Times.Once);
        }
    }
}
