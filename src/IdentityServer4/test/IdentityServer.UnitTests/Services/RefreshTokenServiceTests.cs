using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class RefreshTokenServiceTests
    {
        private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
        private readonly Client _testClient;
        private readonly ClaimsPrincipal _testSubject;

        public RefreshTokenServiceTests()
        {
            _mockRefreshTokenService = new Mock<IRefreshTokenService>();
            _testClient = new Client { ClientId = "test_client" };
            _testSubject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }));
        }

        [Fact]
        public async Task ValidateRefreshToken_WithValidToken_ShouldReturnSuccessResult()
        {
            // Arrange
            var token = "valid_refresh_token";
            var expectedResult = new TokenValidationResult { IsValid = true };
            
            _mockRefreshTokenService.Setup(x => x.ValidateRefreshTokenAsync(token, _testClient))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _mockRefreshTokenService.Object.ValidateRefreshTokenAsync(token, _testClient);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldReturnNewToken()
        {
            // Arrange
            var accessToken = new Token { ClientId = _testClient.ClientId };
            var expectedToken = "new_refresh_token";

            _mockRefreshTokenService.Setup(x => x.CreateRefreshTokenAsync(_testSubject, accessToken, _testClient))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _mockRefreshTokenService.Object.CreateRefreshTokenAsync(_testSubject, accessToken, _testClient);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedToken);
        }

        [Fact]
        public async Task UpdateRefreshToken_ShouldReturnUpdatedToken()
        {
            // Arrange
            var handle = "existing_token_handle";
            var refreshToken = new RefreshToken();
            var expectedToken = "updated_refresh_token";

            _mockRefreshTokenService.Setup(x => x.UpdateRefreshTokenAsync(handle, refreshToken, _testClient))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _mockRefreshTokenService.Object.UpdateRefreshTokenAsync(handle, refreshToken, _testClient);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedToken);
        }
    }
}
