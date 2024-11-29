using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<ITokenService> _mockTokenService;

        public TokenServiceTests()
        {
            _mockTokenService = new Mock<ITokenService>();
        }

        [Fact]
        public async Task CreateIdentityTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var request = new TokenCreationRequest();
            var expectedToken = new Token();
            _mockTokenService.Setup(x => x.CreateIdentityTokenAsync(It.IsAny<TokenCreationRequest>()))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _mockTokenService.Object.CreateIdentityTokenAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result);
        }

        [Fact]
        public async Task CreateAccessTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var request = new TokenCreationRequest();
            var expectedToken = new Token();
            _mockTokenService.Setup(x => x.CreateAccessTokenAsync(It.IsAny<TokenCreationRequest>()))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _mockTokenService.Object.CreateAccessTokenAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result);
        }

        [Fact]
        public async Task CreateSecurityTokenAsync_ShouldReturnSerializedToken()
        {
            // Arrange
            var token = new Token();
            var expectedSerializedToken = "serialized_token";
            _mockTokenService.Setup(x => x.CreateSecurityTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync(expectedSerializedToken);

            // Act
            var result = await _mockTokenService.Object.CreateSecurityTokenAsync(token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSerializedToken, result);
        }
    }
}
