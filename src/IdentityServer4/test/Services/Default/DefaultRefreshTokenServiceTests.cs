using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Services
{
    public class DefaultRefreshTokenServiceTests
    {
        private readonly Mock<IRefreshTokenStore> _mockRefreshTokenStore;
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly Mock<ISystemClock> _mockSystemClock;
        private readonly Mock<ILogger<DefaultRefreshTokenService>> _mockLogger;
        private readonly DefaultRefreshTokenService _service;
        private readonly DateTime _utcNow = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public DefaultRefreshTokenServiceTests()
        {
            _mockRefreshTokenStore = new Mock<IRefreshTokenStore>();
            _mockProfileService = new Mock<IProfileService>();
            _mockSystemClock = new Mock<ISystemClock>();
            _mockLogger = new Mock<ILogger<DefaultRefreshTokenService>>();

            _mockSystemClock.Setup(x => x.UtcNow)
                .Returns(new DateTimeOffset(_utcNow));

            _service = new DefaultRefreshTokenService(
                _mockRefreshTokenStore.Object,
                _mockProfileService.Object,
                _mockSystemClock.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenTokenNotFound_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "invalid_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = true };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenTokenExpired_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "expired_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = true };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow.AddDays(-2),
                Lifetime = 86400, // 1 day in seconds
                ClientId = client.ClientId
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenClientIdMismatch_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "valid_token";
            var client = new Client { ClientId = "different_client", AllowOfflineAccess = true };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow,
                Lifetime = 86400,
                ClientId = "original_client"
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var tokenHandle = "valid_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = true };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow,
                Lifetime = 86400,
                ClientId = client.ClientId,
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            _mockProfileService.Setup(x => x.IsActiveAsync(It.IsAny<IsActiveContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeFalse();
            result.RefreshToken.Should().Be(refreshToken);
            result.Client.Should().Be(client);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenUserNotActive_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "valid_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = true };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow,
                Lifetime = 86400,
                ClientId = client.ClientId,
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            _mockProfileService.Setup(x => x.IsActiveAsync(It.IsAny<IsActiveContext>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenClientOfflineAccessNotAllowed_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "valid_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = false };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow,
                Lifetime = 86400,
                ClientId = client.ClientId
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        public async Task ValidateRefreshToken_WhenTokenConsumed_ShouldReturnInvalidGrant()
        {
            // Arrange
            var tokenHandle = "consumed_token";
            var client = new Client { ClientId = "test_client", AllowOfflineAccess = true };
            var refreshToken = new RefreshToken
            {
                CreationTime = _utcNow,
                Lifetime = 86400,
                ClientId = client.ClientId,
                ConsumedTime = _utcNow
            };

            _mockRefreshTokenStore.Setup(x => x.GetRefreshTokenAsync(tokenHandle))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _service.ValidateRefreshTokenAsync(tokenHandle, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }
    }
}
