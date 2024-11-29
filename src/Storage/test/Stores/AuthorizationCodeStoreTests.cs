using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class AuthorizationCodeStoreTests
    {
        private Mock<IAuthorizationCodeStore> _mockStore;
        private AuthorizationCode _testCode;

        public AuthorizationCodeStoreTests()
        {
            _mockStore = new Mock<IAuthorizationCodeStore>();
            _testCode = new AuthorizationCode
            {
                ClientId = "test_client",
                CreationTime = DateTime.UtcNow,
                Data = "test_data"
            };
        }

        [Fact]
        public async Task StoreAuthorizationCode_ShouldReturnValidCode()
        {
            // Arrange
            var expectedCode = "test_code_123";
            _mockStore.Setup(x => x.StoreAuthorizationCodeAsync(_testCode))
                .ReturnsAsync(expectedCode);

            // Act
            var result = await _mockStore.Object.StoreAuthorizationCodeAsync(_testCode);

            // Assert
            Assert.Equal(expectedCode, result);
            _mockStore.Verify(x => x.StoreAuthorizationCodeAsync(_testCode), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizationCode_ShouldReturnStoredCode()
        {
            // Arrange
            var storedCode = "test_code_123";
            _mockStore.Setup(x => x.GetAuthorizationCodeAsync(storedCode))
                .ReturnsAsync(_testCode);

            // Act
            var result = await _mockStore.Object.GetAuthorizationCodeAsync(storedCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testCode.ClientId, result.ClientId);
            Assert.Equal(_testCode.Data, result.Data);
            _mockStore.Verify(x => x.GetAuthorizationCodeAsync(storedCode), Times.Once);
        }

        [Fact]
        public async Task RemoveAuthorizationCode_ShouldCallRemoveMethod()
        {
            // Arrange
            var codeToRemove = "test_code_123";
            _mockStore.Setup(x => x.RemoveAuthorizationCodeAsync(codeToRemove))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.RemoveAuthorizationCodeAsync(codeToRemove);

            // Assert
            _mockStore.Verify(x => x.RemoveAuthorizationCodeAsync(codeToRemove), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizationCode_ShouldReturnNull_WhenCodeNotFound()
        {
            // Arrange
            var nonExistentCode = "non_existent_code";
            _mockStore.Setup(x => x.GetAuthorizationCodeAsync(nonExistentCode))
                .ReturnsAsync((AuthorizationCode)null);

            // Act
            var result = await _mockStore.Object.GetAuthorizationCodeAsync(nonExistentCode);

            // Assert
            Assert.Null(result);
            _mockStore.Verify(x => x.GetAuthorizationCodeAsync(nonExistentCode), Times.Once);
        }
    }
}
