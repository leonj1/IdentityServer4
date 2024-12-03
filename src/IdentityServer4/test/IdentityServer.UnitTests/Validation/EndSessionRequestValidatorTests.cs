using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Validation
{
    public class EndSessionRequestValidatorTests
    {
        private readonly Mock<ITokenValidator> _tokenValidator;
        private readonly Mock<IRedirectUriValidator> _uriValidator;
        private readonly Mock<IUserSession> _userSession;
        private readonly Mock<ILogoutNotificationService> _logoutNotificationService;
        private readonly Mock<IMessageStore<LogoutNotificationContext>> _endSessionMessageStore;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ILogger<EndSessionRequestValidator>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly EndSessionRequestValidator _validator;

        public EndSessionRequestValidatorTests()
        {
            _tokenValidator = new Mock<ITokenValidator>();
            _uriValidator = new Mock<IRedirectUriValidator>();
            _userSession = new Mock<IUserSession>();
            _logoutNotificationService = new Mock<ILogoutNotificationService>();
            _endSessionMessageStore = new Mock<IMessageStore<LogoutNotificationContext>>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<EndSessionRequestValidator>>();
            _options = new IdentityServerOptions();

            _validator = new EndSessionRequestValidator(
                _httpContextAccessor.Object,
                _options,
                _tokenValidator.Object,
                _uriValidator.Object,
                _userSession.Object,
                _logoutNotificationService.Object,
                _endSessionMessageStore.Object,
                _logger.Object);
        }

        [Fact]
        public async Task ValidateAsync_WhenUserIsAnonymousAndAuthenticationRequired_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var subject = new ClaimsPrincipal(new ClaimsIdentity());
            _options.Authentication.RequireAuthenticatedUserForSignOutMessage = true;

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid request");
            result.ErrorDescription.Should().Contain("User is anonymous");
        }

        [Fact]
        public async Task ValidateAsync_WithValidIdTokenHint_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "valid_token" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }, "test"));

            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("valid_token", null, false))
                .ReturnsAsync(new TokenValidationResult
                {
                    IsError = false,
                    Client = new Client(),
                    Claims = new[] { new Claim(JwtClaimTypes.Subject, "123") }
                });

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateAsync_WithInvalidIdTokenHint_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "invalid_token" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }, "test"));

            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("invalid_token", null, false))
                .ReturnsAsync(new TokenValidationResult { IsError = true });

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid request");
            result.ErrorDescription.Should().Contain("Error validating id token hint");
        }

        [Fact]
        public async Task ValidateAsync_WithMismatchedSubject_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "valid_token" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "456")
            }, "test"));

            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("valid_token", null, false))
                .ReturnsAsync(new TokenValidationResult
                {
                    IsError = false,
                    Client = new Client(),
                    Claims = new[] { new Claim(JwtClaimTypes.Subject, "123") }
                });

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid request");
            result.ErrorDescription.Should().Contain("Current user does not match identity token");
        }

        [Fact]
        public async Task ValidateAsync_WithValidPostLogoutRedirectUri_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "valid_token" },
                { OidcConstants.EndSessionRequest.PostLogoutRedirectUri, "https://valid-uri.com" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }, "test"));

            var client = new Client();
            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("valid_token", null, false))
                .ReturnsAsync(new TokenValidationResult
                {
                    IsError = false,
                    Client = client,
                    Claims = new[] { new Claim(JwtClaimTypes.Subject, "123") }
                });

            _uriValidator.Setup(x => x.IsPostLogoutRedirectUriValid(It.IsAny<string>(), client))
                .Returns(true);

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.PostLogOutUri.Should().Be("https://valid-uri.com");
        }

        [Fact]
        public async Task ValidateAsync_WithInvalidPostLogoutRedirectUri_ShouldNotSetRedirectUri()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "valid_token" },
                { OidcConstants.EndSessionRequest.PostLogoutRedirectUri, "https://invalid-uri.com" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }, "test"));

            var client = new Client();
            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("valid_token", null, false))
                .ReturnsAsync(new TokenValidationResult
                {
                    IsError = false,
                    Client = client,
                    Claims = new[] { new Claim(JwtClaimTypes.Subject, "123") }
                });

            _uriValidator.Setup(x => x.IsPostLogoutRedirectUriValid(It.IsAny<string>(), client))
                .Returns(false);

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.PostLogOutUri.Should().BeNull();
        }

        [Fact]
        public async Task ValidateAsync_WithStateParameter_ShouldOnlyIncludeStateWithValidRedirectUri()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.EndSessionRequest.IdTokenHint, "valid_token" },
                { OidcConstants.EndSessionRequest.PostLogoutRedirectUri, "https://valid-uri.com" },
                { OidcConstants.EndSessionRequest.State, "test_state" }
            };
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "123")
            }, "test"));

            var client = new Client();
            _tokenValidator.Setup(x => x.ValidateIdentityTokenAsync("valid_token", null, false))
                .ReturnsAsync(new TokenValidationResult
                {
                    IsError = false,
                    Client = client,
                    Claims = new[] { new Claim(JwtClaimTypes.Subject, "123") }
                });

            _uriValidator.Setup(x => x.IsPostLogoutRedirectUriValid(It.IsAny<string>(), client))
                .Returns(true);

            // Act
            var result = await _validator.ValidateAsync(parameters, subject);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.State.Should().Be("test_state");
        }
    }
}
