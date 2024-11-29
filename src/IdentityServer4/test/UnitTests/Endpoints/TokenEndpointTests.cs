using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Endpoints
{
    public class TokenEndpointTests
    {
        private readonly Mock<IClientSecretValidator> _clientValidator;
        private readonly Mock<ITokenRequestValidator> _requestValidator;
        private readonly Mock<ITokenResponseGenerator> _responseGenerator;
        private readonly Mock<IEventService> _events;
        private readonly Mock<ILogger<TokenEndpoint>> _logger;
        private readonly TokenEndpoint _subject;

        public TokenEndpointTests()
        {
            _clientValidator = new Mock<IClientSecretValidator>();
            _requestValidator = new Mock<ITokenRequestValidator>();
            _responseGenerator = new Mock<ITokenResponseGenerator>();
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<TokenEndpoint>>();

            _subject = new TokenEndpoint(
                _clientValidator.Object,
                _requestValidator.Object,
                _responseGenerator.Object,
                _events.Object,
                _logger.Object);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_http_method_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET"; // Invalid method, should be POST

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            var tokenResult = result.Should().BeOfType<TokenErrorResult>().Subject;
            tokenResult.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_content_type_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json"; // Invalid content type

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            var tokenResult = result.Should().BeOfType<TokenErrorResult>().Subject;
            tokenResult.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task ProcessAsync_with_valid_request_should_return_token_response()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var client = new Client { ClientId = "client" };
            var clientResult = new ClientSecretValidationResult { Client = client };
            _clientValidator.Setup(x => x.ValidateAsync(context))
                .ReturnsAsync(clientResult);

            var validationResult = new TokenRequestValidationResult(new ValidatedTokenRequest());
            _requestValidator.Setup(x => x.ValidateRequestAsync(It.IsAny<NameValueCollection>(), clientResult))
                .ReturnsAsync(validationResult);

            var responseResult = new TokenResponse();
            _responseGenerator.Setup(x => x.ProcessAsync(validationResult))
                .ReturnsAsync(responseResult);

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<TokenResult>();
            var tokenResult = (TokenResult)result;
            tokenResult.Response.Should().Be(responseResult);

            _events.Verify(x => x.RaiseAsync(It.IsAny<TokenIssuedSuccessEvent>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_with_invalid_client_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var clientResult = new ClientSecretValidationResult { Client = null };
            _clientValidator.Setup(x => x.ValidateAsync(context))
                .ReturnsAsync(clientResult);

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            var tokenResult = result.Should().BeOfType<TokenErrorResult>().Subject;
            tokenResult.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
            _events.Verify(x => x.RaiseAsync(It.IsAny<TokenIssuedFailureEvent>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_with_token_validation_failure_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var client = new Client { ClientId = "client" };
            var clientResult = new ClientSecretValidationResult { Client = client };
            _clientValidator.Setup(x => x.ValidateAsync(context))
                .ReturnsAsync(clientResult);

            var validationResult = new TokenRequestValidationResult(new ValidatedTokenRequest())
            {
                IsError = true,
                Error = "validation_error"
            };
            _requestValidator.Setup(x => x.ValidateRequestAsync(It.IsAny<NameValueCollection>(), clientResult))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            var tokenResult = result.Should().BeOfType<TokenErrorResult>().Subject;
            tokenResult.Response.Error.Should().Be("validation_error");
            _events.Verify(x => x.RaiseAsync(It.IsAny<TokenIssuedFailureEvent>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_with_token_generation_failure_should_return_error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var client = new Client { ClientId = "client" };
            var clientResult = new ClientSecretValidationResult { Client = client };
            _clientValidator.Setup(x => x.ValidateAsync(context))
                .ReturnsAsync(clientResult);

            var validationResult = new TokenRequestValidationResult(new ValidatedTokenRequest());
            _requestValidator.Setup(x => x.ValidateRequestAsync(It.IsAny<NameValueCollection>(), clientResult))
                .ReturnsAsync(validationResult);

            _responseGenerator.Setup(x => x.ProcessAsync(validationResult))
                .ThrowsAsync(new Exception("token generation failed"));

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            var tokenResult = result.Should().BeOfType<TokenErrorResult>().Subject;
            tokenResult.Response.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
            _events.Verify(x => x.RaiseAsync(It.IsAny<TokenIssuedFailureEvent>()), Times.Once);
        }
    }
}
