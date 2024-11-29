using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.ResponseHandling
{
    public class TokenRevocationResponseGeneratorTests
    {
        private readonly Mock<IReferenceTokenStore> _mockReferenceTokenStore;
        private readonly Mock<IRefreshTokenStore> _mockRefreshTokenStore;
        private readonly Mock<ILogger<TokenRevocationResponseGenerator>> _mockLogger;
        private readonly TokenRevocationResponseGenerator _generator;
        private readonly Client _client;

        public TokenRevocationResponseGeneratorTests()
        {
            _mockReferenceTokenStore = new Mock<IReferenceTokenStore>();
            _mockRefreshTokenStore = new Mock<IRefreshTokenStore>();
            _mockLogger = new Mock<ILogger<TokenRevocationResponseGenerator>>();
            _generator = new TokenRevocationResponseGenerator(
                _mockReferenceTokenStore.Object,
                _mockRefreshTokenStore.Object,
                _mockLogger.Object);

            _client = new Client
            {
                ClientId = "test_client"
            };
        }

        [Fact]
        public async Task When_RevokeAccessToken_With_Valid_Token_Should_Succeed()
        {
            // Arrange
            var token = new Token
            {
                ClientId = "test_client"
            };
            
            _mockReferenceTokenStore.Setup(x => x.GetReferenceTokenAsync("valid_token"))
                .ReturnsAsync(token);

            var validationResult = new TokenRevocationRequestValidationResult
            {
                Token = "valid_token",
                TokenTypeHint = Constants.TokenTypeHints.AccessToken,
                Client = _client
            };

            // Act
            var response = await _generator.ProcessAsync(validationResult);

            // Assert
            response.Success.Should().BeTrue();
            response.TokenType.Should().Be(Constants.TokenTypeHints.AccessToken);
            _mockReferenceTokenStore.Verify(x => x.RemoveReferenceTokenAsync("valid_token"), Times.Once);
        }

        [Fact]
        public async Task When_RevokeRefreshToken_With_Valid_Token_Should_Succeed()
        {
            // Arrange
            var token = new RefreshToken
            {
                ClientId = "test_client",
                SubjectId = "subject"
            };
            
            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync("valid_refresh_token"))
                .ReturnsAsync(token);

            var validationResult = new TokenRevocationRequestValidationResult
            {
                Token = "valid_refresh_token",
                TokenTypeHint = Constants.TokenTypeHints.RefreshToken,
                Client = _client
            };

            // Act
            var response = await _generator.ProcessAsync(validationResult);

            // Assert
            response.Success.Should().BeTrue();
            response.TokenType.Should().Be(Constants.TokenTypeHints.RefreshToken);
            _mockRefreshTokenStore.Verify(x => x.RemoveRefreshTokenAsync("valid_refresh_token"), Times.Once);
            _mockReferenceTokenStore.Verify(x => x.RemoveReferenceTokensAsync("subject", "test_client"), Times.Once);
        }

        [Fact]
        public async Task When_RevokeToken_With_Wrong_ClientId_Should_Not_Revoke()
        {
            // Arrange
            var token = new Token
            {
                ClientId = "different_client"
            };
            
            _mockReferenceTokenStore.Setup(x => x.GetReferenceTokenAsync("valid_token"))
                .ReturnsAsync(token);

            var validationResult = new TokenRevocationRequestValidationResult
            {
                Token = "valid_token",
                TokenTypeHint = Constants.TokenTypeHints.AccessToken,
                Client = _client
            };

            // Act
            var response = await _generator.ProcessAsync(validationResult);

            // Assert
            response.Success.Should().BeTrue(); // API always returns true for security reasons
            _mockReferenceTokenStore.Verify(x => x.RemoveReferenceTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task When_RevokeToken_Without_Hint_Should_Try_Both_Types()
        {
            // Arrange
            _mockReferenceTokenStore.Setup(x => x.GetReferenceTokenAsync("token"))
                .ReturnsAsync((Token)null);
            
            var refreshToken = new RefreshToken
            {
                ClientId = "test_client",
                SubjectId = "subject"
            };
            
            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync("token"))
                .ReturnsAsync(refreshToken);

            var validationResult = new TokenRevocationRequestValidationResult
            {
                Token = "token",
                TokenTypeHint = null,
                Client = _client
            };

            // Act
            var response = await _generator.ProcessAsync(validationResult);

            // Assert
            response.Success.Should().BeTrue();
            response.TokenType.Should().Be(Constants.TokenTypeHints.RefreshToken);
            _mockRefreshTokenStore.Verify(x => x.RemoveRefreshTokenAsync("token"), Times.Once);
            _mockReferenceTokenStore.Verify(x => x.RemoveReferenceTokensAsync("subject", "test_client"), Times.Once);
        }
    }
}
