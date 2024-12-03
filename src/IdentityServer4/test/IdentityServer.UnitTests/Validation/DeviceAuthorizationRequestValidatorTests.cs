using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class DeviceAuthorizationRequestValidatorTests
    {
        private readonly IdentityServerOptions _options;
        private readonly IResourceValidator _resourceValidator;
        private readonly ILogger<DeviceAuthorizationRequestValidator> _logger;
        private readonly DeviceAuthorizationRequestValidator _validator;

        public DeviceAuthorizationRequestValidatorTests()
        {
            _options = new IdentityServerOptions();
            _resourceValidator = new StubResourceValidator();
            _logger = new LoggerFactory().CreateLogger<DeviceAuthorizationRequestValidator>();
            _validator = new DeviceAuthorizationRequestValidator(_options, _resourceValidator, _logger);
        }

        [Fact]
        public async Task ValidateAsync_WhenParametersNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client { ClientId = "client" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateAsync(null, clientValidationResult));
        }

        [Fact]
        public async Task ValidateAsync_WhenClientValidationResultNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var parameters = new NameValueCollection();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateAsync(parameters, null));
        }

        [Fact]
        public async Task ValidateAsync_WhenClientNotAllowedDeviceFlow_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { GrantType.ClientCredentials } // Not device flow
                }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.UnauthorizedClient);
        }

        [Fact]
        public async Task ValidateAsync_WhenValidRequest_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.Scope, "openid profile" }
            };
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { GrantType.DeviceFlow }
                }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenScopeMissing_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { GrantType.DeviceFlow }
                }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidScope);
        }

        [Fact]
        public async Task ValidateAsync_WhenInvalidProtocolType_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.AuthorizeRequest.Scope, "openid profile" }
            };
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = "invalid_protocol",
                    AllowedGrantTypes = { GrantType.DeviceFlow }
                }
            };

            // Act
            var result = await _validator.ValidateAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }
    }

    internal class StubResourceValidator : IResourceValidator
    {
        public Task<ResourceValidationResult> ValidateRequestedResourcesAsync(ResourceValidationRequest request)
        {
            return Task.FromResult(new ResourceValidationResult());
        }
    }
}
