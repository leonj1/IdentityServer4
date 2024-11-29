using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Endpoints
{
    public class DeviceAuthorizationEndpointTests
    {
        private readonly Mock<IClientSecretValidator> _clientValidator;
        private readonly Mock<IDeviceAuthorizationRequestValidator> _requestValidator;
        private readonly Mock<IDeviceAuthorizationResponseGenerator> _responseGenerator;
        private readonly Mock<IEventService> _events;
        private readonly ILogger<DeviceAuthorizationEndpoint> _logger;
        private readonly DeviceAuthorizationEndpoint _subject;
        private readonly HttpContext _context;

        public DeviceAuthorizationEndpointTests()
        {
            _clientValidator = new Mock<IClientSecretValidator>();
            _requestValidator = new Mock<IDeviceAuthorizationRequestValidator>();
            _responseGenerator = new Mock<IDeviceAuthorizationResponseGenerator>();
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<DeviceAuthorizationEndpoint>>().Object;

            _subject = new DeviceAuthorizationEndpoint(
                _clientValidator.Object,
                _requestValidator.Object,
                _responseGenerator.Object,
                _events.Object,
                _logger
            );

            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_http_method_should_return_error()
        {
            // Arrange
            _context.Request.Method = "GET";

            // Act
            var result = await _subject.ProcessAsync(_context);

            // Assert
            var tokenError = result as TokenErrorResult;
            tokenError.Should().NotBeNull();
            tokenError.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_content_type_should_return_error()
        {
            // Arrange
            _context.Request.Method = "POST";
            _context.Request.ContentType = "application/json";

            // Act
            var result = await _subject.ProcessAsync(_context);

            // Assert
            var tokenError = result as TokenErrorResult;
            tokenError.Should().NotBeNull();
            tokenError.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_client_should_return_error()
        {
            // Arrange
            _context.Request.Method = "POST";
            _context.Request.ContentType = "application/x-www-form-urlencoded";
            
            _clientValidator.Setup(v => v.ValidateAsync(_context))
                .ReturnsAsync(new ClientSecretValidationResult { Client = null });

            // Act
            var result = await _subject.ProcessAsync(_context);

            // Assert
            var tokenError = result as TokenErrorResult;
            tokenError.Should().NotBeNull();
            tokenError.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
        }

        [Fact]
        public async Task ProcessAsync_with_valid_request_should_return_success()
        {
            // Arrange
            _context.Request.Method = "POST";
            _context.Request.ContentType = "application/x-www-form-urlencoded";
            
            var client = new Client { ClientId = "client" };
            _clientValidator.Setup(v => v.ValidateAsync(_context))
                .ReturnsAsync(new ClientSecretValidationResult { Client = client });

            var validationResult = new DeviceAuthorizationRequestValidationResult
            {
                ValidatedRequest = new ValidatedDeviceAuthorizationRequest
                {
                    Client = client
                }
            };
            _requestValidator.Setup(v => v.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClientSecretValidationResult>()))
                .ReturnsAsync(validationResult);

            var response = new DeviceAuthorizationResponse
            {
                DeviceCode = "device_code",
                UserCode = "user_code",
                VerificationUri = "verification_uri",
                VerificationUriComplete = "verification_uri_complete"
            };
            _responseGenerator.Setup(g => g.ProcessAsync(It.IsAny<DeviceAuthorizationRequestValidationResult>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _subject.ProcessAsync(_context);

            // Assert
            var deviceResult = result as DeviceAuthorizationResult;
            deviceResult.Should().NotBeNull();
            deviceResult.Response.Should().Be(response);
        }
    }
}
