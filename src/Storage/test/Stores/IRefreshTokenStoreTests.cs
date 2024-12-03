using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class IRefreshTokenStoreTests
    {
        private Mock<IRefreshTokenStore> _mockStore;
        private RefreshToken _testRefreshToken;

        public IRefreshTokenStoreTests()
        {
            _mockStore = new Mock<IRefreshTokenStore>();
            _testRefreshToken = new RefreshToken
            {
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600,
                SubjectId = "test_subject",
                ClientId = "test_client"
            };
        }

        [Fact]
        public async Task StoreRefreshToken_ShouldReturnHandle()
        {
            // Arrange
            var expectedHandle = "test_handle";
            _mockStore.Setup(x => x.StoreRefreshTokenAsync(_testRefreshToken))
                .ReturnsAsync(expectedHandle);

            // Act
            var result = await _mockStore.Object.StoreRefreshTokenAsync(_testRefreshToken);

            // Assert
            Assert.Equal(expectedHandle, result);
            _mockStore.Verify(x => x.StoreRefreshTokenAsync(_testRefreshToken), Times.Once);
        }

        [Fact]
        public async Task GetRefreshToken_ShouldReturnStoredToken()
        {
            // Arrange
            var handle = "test_handle";
            _mockStore.Setup(x => x.GetRefreshTokenAsync(handle))
                .ReturnsAsync(_testRefreshToken);

            // Act
            var result = await _mockStore.Object.GetRefreshTokenAsync(handle);

            // Assert
            Assert.Equal(_testRefreshToken, result);
            _mockStore.Verify(x => x.GetRefreshTokenAsync(handle), Times.Once);
        }

        [Fact]
        public async Task UpdateRefreshToken_ShouldCallUpdateMethod()
        {
            // Arrange
            var handle = "test_handle";
            _mockStore.Setup(x => x.UpdateRefreshTokenAsync(handle, _testRefreshToken))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.UpdateRefreshTokenAsync(handle, _testRefreshToken);

            // Assert
            _mockStore.Verify(x => x.UpdateRefreshTokenAsync(handle, _testRefreshToken), Times.Once);
        }

        [Fact]
        public async Task RemoveRefreshToken_ShouldCallRemoveMethod()
        {
            // Arrange
            var handle = "test_handle";
            _mockStore.Setup(x => x.RemoveRefreshTokenAsync(handle))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.RemoveRefreshTokenAsync(handle);

            // Assert
            _mockStore.Verify(x => x.RemoveRefreshTokenAsync(handle), Times.Once);
        }

        [Fact]
        public async Task RemoveRefreshTokens_ShouldCallRemoveMethodWithSubjectAndClientId()
        {
            // Arrange
            var subjectId = "test_subject";
            var clientId = "test_client";
            _mockStore.Setup(x => x.RemoveRefreshTokensAsync(subjectId, clientId))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.RemoveRefreshTokensAsync(subjectId, clientId);

            // Assert
            _mockStore.Verify(x => x.RemoveRefreshTokensAsync(subjectId, clientId), Times.Once);
        }
    }
}
