using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultTokenServiceTests
    {
        private readonly DefaultTokenService _sut;
        private readonly Mock<IClaimsService> _claimsProvider;
        private readonly Mock<IReferenceTokenStore> _referenceTokenStore;
        private readonly Mock<ITokenCreationService> _creationService;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<IKeyMaterialService> _keyMaterialService;
        private readonly IdentityServerOptions _options;
        private readonly Mock<ILogger<DefaultTokenService>> _logger;

        public DefaultTokenServiceTests()
        {
            _claimsProvider = new Mock<IClaimsService>();
            _referenceTokenStore = new Mock<IReferenceTokenStore>();
            _creationService = new Mock<ITokenCreationService>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _clock = new Mock<ISystemClock>();
            _keyMaterialService = new Mock<IKeyMaterialService>();
            _options = new IdentityServerOptions();
            _logger = new Mock<ILogger<DefaultTokenService>>();

            _contextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            _sut = new DefaultTokenService(
                _claimsProvider.Object,
                _referenceTokenStore.Object,
                _creationService.Object,
                _contextAccessor.Object,
                _clock.Object,
                _keyMaterialService.Object,
                _options,
                _logger.Object);
        }

        [Fact]
        public async Task CreateIdentityTokenAsync_ShouldThrowException_WhenRequestIsNull()
        {
            // Arrange
            TokenCreationRequest request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _sut.CreateIdentityTokenAsync(request));
        }

        [Fact]
        public async Task CreateIdentityTokenAsync_ShouldCreateValidToken_WithBasicClaims()
        {
            // Arrange
            var request = new TokenCreationRequest
            {
                ValidatedRequest = new ValidatedRequest
                {
                    Client = new Client { ClientId = "test_client" },
                    SessionId = "test_session"
                },
                Subject = new ClaimsPrincipal(new ClaimsIdentity()),
                ValidatedResources = new ResourceValidationResult()
            };

            _keyMaterialService.Setup(x => x.GetSigningCredentialsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new SigningCredentials(null, "RS256"));

            // Act
            var token = await _sut.CreateIdentityTokenAsync(request);

            // Assert
            token.Should().NotBeNull();
            token.Type.Should().Be(OidcConstants.TokenTypes.IdentityToken);
            token.ClientId.Should().Be("test_client");
            token.Claims.Should().Contain(x => x.Type == JwtClaimTypes.SessionId);
        }

        [Fact]
        public async Task CreateAccessTokenAsync_ShouldCreateValidToken_WithBasicClaims()
        {
            // Arrange
            var request = new TokenCreationRequest
            {
                ValidatedRequest = new ValidatedRequest
                {
                    Client = new Client 
                    { 
                        ClientId = "test_client",
                        IncludeJwtId = true
                    },
                    AccessTokenLifetime = 3600,
                    SessionId = "test_session"
                },
                Subject = new ClaimsPrincipal(new ClaimsIdentity()),
                ValidatedResources = new ResourceValidationResult()
            };

            // Act
            var token = await _sut.CreateAccessTokenAsync(request);

            // Assert
            token.Should().NotBeNull();
            token.Type.Should().Be(OidcConstants.TokenTypes.AccessToken);
            token.ClientId.Should().Be("test_client");
            token.Lifetime.Should().Be(3600);
            token.Claims.Should().Contain(x => x.Type == JwtClaimTypes.JwtId);
            token.Claims.Should().Contain(x => x.Type == JwtClaimTypes.SessionId);
        }

        [Fact]
        public async Task CreateSecurityTokenAsync_ShouldCreateJwtToken_WhenAccessTokenTypeIsJwt()
        {
            // Arrange
            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                AccessTokenType = AccessTokenType.Jwt,
                ClientId = "test_client"
            };

            _creationService.Setup(x => x.CreateTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync("jwt_token");

            // Act
            var result = await _sut.CreateSecurityTokenAsync(token);

            // Assert
            result.Should().Be("jwt_token");
            _creationService.Verify(x => x.CreateTokenAsync(It.IsAny<Token>()), Times.Once);
        }

        [Fact]
        public async Task CreateSecurityTokenAsync_ShouldCreateReferenceToken_WhenAccessTokenTypeIsReference()
        {
            // Arrange
            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                AccessTokenType = AccessTokenType.Reference,
                ClientId = "test_client"
            };

            _referenceTokenStore.Setup(x => x.StoreReferenceTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync("reference_token");

            // Act
            var result = await _sut.CreateSecurityTokenAsync(token);

            // Assert
            result.Should().Be("reference_token");
            _referenceTokenStore.Verify(x => x.StoreReferenceTokenAsync(It.IsAny<Token>()), Times.Once);
        }
    }
}
