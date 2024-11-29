using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;

namespace IdentityServer.UnitTests.Validation
{
    public class AuthorizeRequestValidatorTests
    {
        private readonly ILogger<AuthorizeRequestValidator> _logger;
        private readonly IdentityServerOptions _options;
        private readonly IClientStore _clientStore;
        private readonly ICustomAuthorizeRequestValidator _customValidator;
        private readonly IRedirectUriValidator _uriValidator;
        private readonly IResourceValidator _resourceValidator;
        private readonly IUserSession _userSession;
        private readonly JwtRequestValidator _jwtRequestValidator;
        private readonly IJwtRequestUriHttpClient _jwtRequestUriHttpClient;

        public AuthorizeRequestValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<AuthorizeRequestValidator>();
            _options = new IdentityServerOptions();
            _clientStore = new InMemoryClientStore(new List<Client>());
            _customValidator = new DefaultCustomAuthorizeRequestValidator();
            _uriValidator = new StrictRedirectUriValidator();
            _resourceValidator = new DefaultResourceValidator(new InMemoryResourcesStore(new List<IdentityResource>()));
            _userSession = new DefaultUserSession(new HttpContextAccessor(), new ClaimsService(), _options);
            _jwtRequestValidator = new JwtRequestValidator(_options, _clientStore);
            _jwtRequestUriHttpClient = new DefaultJwtRequestUriHttpClient();
        }

        [Fact]
        public async Task ValidateAsync_WhenParametersNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var validator = new AuthorizeRequestValidator(
                _options,
                _clientStore,
                _customValidator,
                _uriValidator,
                _resourceValidator,
                _userSession,
                _jwtRequestValidator,
                _jwtRequestUriHttpClient,
                _logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                validator.ValidateAsync(null));
        }

        [Fact]
        public async Task ValidateAsync_WhenClientIdMissing_ShouldReturnError()
        {
            // Arrange
            var validator = new AuthorizeRequestValidator(
                _options,
                _clientStore,
                _customValidator,
                _uriValidator,
                _resourceValidator,
                _userSession,
                _jwtRequestValidator,
                _jwtRequestUriHttpClient,
                _logger);

            var parameters = new NameValueCollection();

            // Act
            var result = await validator.ValidateAsync(parameters);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid client_id");
        }

        [Fact]
        public async Task ValidateAsync_WhenClientIdTooLong_ShouldReturnError()
        {
            // Arrange
            var validator = new AuthorizeRequestValidator(
                _options,
                _clientStore,
                _customValidator,
                _uriValidator,
                _resourceValidator,
                _userSession,
                _jwtRequestValidator,
                _jwtRequestUriHttpClient,
                _logger);

            var parameters = new NameValueCollection
            {
                { "client_id", new string('x', _options.InputLengthRestrictions.ClientId + 1) }
            };

            // Act
            var result = await validator.ValidateAsync(parameters);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid client_id");
        }

        [Fact]
        public async Task ValidateAsync_WhenClientNotFound_ShouldReturnError()
        {
            // Arrange
            var validator = new AuthorizeRequestValidator(
                _options,
                _clientStore,
                _customValidator,
                _uriValidator,
                _resourceValidator,
                _userSession,
                _jwtRequestValidator,
                _jwtRequestUriHttpClient,
                _logger);

            var parameters = new NameValueCollection
            {
                { "client_id", "unknown_client" }
            };

            // Act
            var result = await validator.ValidateAsync(parameters);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("unauthorized_client");
        }
    }
}
