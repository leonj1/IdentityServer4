using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class TokenRequestValidatorTests
    {
        private readonly TokenRequestValidator _validator;
        private readonly Mock<IAuthorizationCodeStore> _mockAuthorizationCodeStore;
        private readonly Mock<IResourceOwnerPasswordValidator> _mockResourceOwnerValidator;
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly Mock<IDeviceCodeValidator> _mockDeviceCodeValidator;
        private readonly Mock<ICustomTokenRequestValidator> _mockCustomRequestValidator;
        private readonly Mock<IResourceValidator> _mockResourceValidator;
        private readonly Mock<IResourceStore> _mockResourceStore;
        private readonly Mock<ITokenValidator> _mockTokenValidator;
        private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ISystemClock> _mockSystemClock;
        private readonly IdentityServerOptions _options;

        public TokenRequestValidatorTests()
        {
            _mockAuthorizationCodeStore = new Mock<IAuthorizationCodeStore>();
            _mockResourceOwnerValidator = new Mock<IResourceOwnerPasswordValidator>();
            _mockProfileService = new Mock<IProfileService>();
            _mockDeviceCodeValidator = new Mock<IDeviceCodeValidator>();
            _mockCustomRequestValidator = new Mock<ICustomTokenRequestValidator>();
            _mockResourceValidator = new Mock<IResourceValidator>();
            _mockResourceStore = new Mock<IResourceStore>();
            _mockTokenValidator = new Mock<ITokenValidator>();
            _mockRefreshTokenService = new Mock<IRefreshTokenService>();
            _mockEventService = new Mock<IEventService>();
            _mockSystemClock = new Mock<ISystemClock>();
            
            _options = new IdentityServerOptions();

            _validator = new TokenRequestValidator(
                _options,
                _mockAuthorizationCodeStore.Object,
                _mockResourceOwnerValidator.Object,
                _mockProfileService.Object,
                _mockDeviceCodeValidator.Object,
                new ExtensionGrantValidator(new List<IExtensionGrantValidator>()),
                _mockCustomRequestValidator.Object,
                _mockResourceValidator.Object,
                _mockResourceStore.Object,
                _mockTokenValidator.Object,
                _mockRefreshTokenService.Object,
                _mockEventService.Object,
                _mockSystemClock.Object,
                Mock.Of<ILogger<TokenRequestValidator>>());
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenParametersNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            NameValueCollection parameters = null;
            var clientValidationResult = new ClientSecretValidationResult 
            { 
                Client = new Client { ClientId = "client" },
                Secret = "secret"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestAsync(parameters, clientValidationResult));
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenClientValidationResultNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var parameters = new NameValueCollection();
            ClientSecretValidationResult clientValidationResult = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestAsync(parameters, clientValidationResult));
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenInvalidProtocolType_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = "invalid"
                },
                Secret = "secret"
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenMissingGrantType_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect
                },
                Secret = "secret"
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenGrantTypeTooLong_ShouldReturnError()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { OidcConstants.TokenRequest.GrantType, new string('x', _options.InputLengthRestrictions.GrantType + 1) }
            };
            var clientValidationResult = new ClientSecretValidationResult
            {
                Client = new Client 
                { 
                    ClientId = "client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect
                },
                Secret = "secret"
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, clientValidationResult);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
        }
    }
}
