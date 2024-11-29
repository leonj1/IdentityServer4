using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using IdentityServer4.Services;
using IdentityModel;

namespace IdentityServer4.UnitTests.Endpoints
{
    public class TokenRevocationEndpointTests
    {
        private readonly ILogger<TokenRevocationEndpoint> _logger;
        private readonly IClientSecretValidator _clientValidator;
        private readonly ITokenRevocationRequestValidator _requestValidator;
        private readonly ITokenRevocationResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly TokenRevocationEndpoint _target;

        public TokenRevocationEndpointTests()
        {
            _logger = new FakeLogger<TokenRevocationEndpoint>();
            _clientValidator = new MockClientSecretValidator();
            _requestValidator = new MockTokenRevocationRequestValidator();
            _responseGenerator = new MockTokenRevocationResponseGenerator();
            _events = new MockEventService();
            
            _target = new TokenRevocationEndpoint(
                _logger,
                _clientValidator,
                _requestValidator,
                _responseGenerator,
                _events);
        }

        [Fact]
        public async Task ProcessAsync_WhenMethodNotPost_ShouldReturnMethodNotAllowed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";

            // Act
            var result = await _target.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task ProcessAsync_WhenInvalidContentType_ShouldReturnUnsupportedMediaType()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";

            // Act
            var result = await _target.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task ProcessAsync_WhenValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            // Act
            var result = await _target.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    internal class MockClientSecretValidator : IClientSecretValidator
    {
        public Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            return Task.FromResult(new ClientSecretValidationResult
            {
                IsError = false,
                Client = new Client()
            });
        }
    }

    internal class MockTokenRevocationRequestValidator : ITokenRevocationRequestValidator
    {
        public Task<TokenRevocationRequestValidationResult> ValidateRequestAsync(System.Collections.Specialized.NameValueCollection parameters, Client client)
        {
            return Task.FromResult(new TokenRevocationRequestValidationResult
            {
                IsError = false,
                Client = client
            });
        }
    }

    internal class MockTokenRevocationResponseGenerator : ITokenRevocationResponseGenerator
    {
        public Task<TokenRevocationResponse> ProcessAsync(TokenRevocationRequestValidationResult validationResult)
        {
            return Task.FromResult(new TokenRevocationResponse
            {
                Success = true,
                Error = null
            });
        }
    }

    internal class MockEventService : IEventService
    {
        public Task RaiseAsync(Event evt)
        {
            return Task.CompletedTask;
        }
    }
}
