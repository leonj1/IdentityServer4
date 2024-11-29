using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class AuthorizeResponseGeneratorTests
    {
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IKeyMaterialService> _keyMaterialService;
        private readonly Mock<IAuthorizationCodeStore> _authorizationCodeStore;
        private readonly Mock<ILogger<AuthorizeResponseGenerator>> _logger;
        private readonly Mock<IEventService> _eventService;
        private readonly AuthorizeResponseGenerator _generator;
        private readonly ValidatedAuthorizeRequest _validRequest;

        public AuthorizeResponseGeneratorTests()
        {
            _clock = new Mock<ISystemClock>();
            _tokenService = new Mock<ITokenService>();
            _keyMaterialService = new Mock<IKeyMaterialService>();
            _authorizationCodeStore = new Mock<IAuthorizationCodeStore>();
            _logger = new Mock<ILogger<AuthorizeResponseGenerator>>();
            _eventService = new Mock<IEventService>();

            _generator = new AuthorizeResponseGenerator(
                _clock.Object,
                _tokenService.Object,
                _keyMaterialService.Object,
                _authorizationCodeStore.Object,
                _logger.Object,
                _eventService.Object
            );

            _validRequest = new ValidatedAuthorizeRequest()
            {
                ClientId = "test_client",
                RedirectUri = "https://test.com/callback",
                State = "test_state",
                Client = new Client()
                {
                    ClientId = "test_client",
                    AuthorizationCodeLifetime = 300
                }
            };
        }

        [Fact]
        public async Task CreateResponseAsync_WithCodeGrantType_ShouldReturnCodeResponse()
        {
            // Arrange
            _validRequest.GrantType = GrantType.AuthorizationCode;
            var code = new AuthorizationCode();
            var codeId = "test_code_id";

            _authorizationCodeStore
                .Setup(x => x.StoreAuthorizationCodeAsync(It.IsAny<AuthorizationCode>()))
                .ReturnsAsync(codeId);

            // Act
            var response = await _generator.CreateResponseAsync(_validRequest);

            // Assert
            response.Should().NotBeNull();
            response.Code.Should().Be(codeId);
            response.Request.Should().Be(_validRequest);
        }

        [Fact]
        public async Task CreateResponseAsync_WithImplicitGrantType_ShouldReturnImplicitResponse()
        {
            // Arrange
            _validRequest.GrantType = GrantType.Implicit;
            _validRequest.ResponseType = "token";
            var accessToken = new Token { Lifetime = 3600 };
            var accessTokenValue = "test_access_token";

            _tokenService
                .Setup(x => x.CreateAccessTokenAsync(It.IsAny<TokenCreationRequest>()))
                .ReturnsAsync(accessToken);
            _tokenService
                .Setup(x => x.CreateSecurityTokenAsync(accessToken))
                .ReturnsAsync(accessTokenValue);

            // Act
            var response = await _generator.CreateResponseAsync(_validRequest);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().Be(accessTokenValue);
            response.AccessTokenLifetime.Should().Be(3600);
        }

        [Fact]
        public async Task CreateResponseAsync_WithHybridGrantType_ShouldReturnHybridResponse()
        {
            // Arrange
            _validRequest.GrantType = GrantType.Hybrid;
            _validRequest.ResponseType = "code token";
            var code = new AuthorizationCode();
            var codeId = "test_code_id";
            var accessToken = new Token { Lifetime = 3600 };
            var accessTokenValue = "test_access_token";

            _authorizationCodeStore
                .Setup(x => x.StoreAuthorizationCodeAsync(It.IsAny<AuthorizationCode>()))
                .ReturnsAsync(codeId);
            _tokenService
                .Setup(x => x.CreateAccessTokenAsync(It.IsAny<TokenCreationRequest>()))
                .ReturnsAsync(accessToken);
            _tokenService
                .Setup(x => x.CreateSecurityTokenAsync(accessToken))
                .ReturnsAsync(accessTokenValue);

            // Act
            var response = await _generator.CreateResponseAsync(_validRequest);

            // Assert
            response.Should().NotBeNull();
            response.Code.Should().Be(codeId);
            response.AccessToken.Should().Be(accessTokenValue);
            response.AccessTokenLifetime.Should().Be(3600);
        }

        [Fact]
        public async Task CreateResponseAsync_WithInvalidGrantType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _validRequest.GrantType = "invalid_grant_type";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _generator.CreateResponseAsync(_validRequest));
        }

        [Fact]
        public async Task CreateResponseAsync_WithNullRequest_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _generator.CreateResponseAsync(null));
        }
    }
}
