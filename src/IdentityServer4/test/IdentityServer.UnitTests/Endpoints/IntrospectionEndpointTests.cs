using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class IntrospectionEndpointTests
    {
        private readonly Mock<IApiSecretValidator> _apiSecretValidator;
        private readonly Mock<IIntrospectionRequestValidator> _requestValidator;
        private readonly Mock<IIntrospectionResponseGenerator> _responseGenerator;
        private readonly Mock<IEventService> _events;
        private readonly Mock<ILogger<IntrospectionEndpoint>> _logger;
        private readonly IntrospectionEndpoint _endpoint;

        public IntrospectionEndpointTests()
        {
            _apiSecretValidator = new Mock<IApiSecretValidator>();
            _requestValidator = new Mock<IIntrospectionRequestValidator>();
            _responseGenerator = new Mock<IIntrospectionResponseGenerator>();
            _events = new Mock<IEventService>();
            _logger = new Mock<ILogger<IntrospectionEndpoint>>();

            _endpoint = new IntrospectionEndpoint(
                _apiSecretValidator.Object,
                _requestValidator.Object,
                _responseGenerator.Object,
                _events.Object,
                _logger.Object);
        }

        [Fact]
        public async Task GetRequest_ShouldReturn_MethodNotAllowed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";

            // Act
            var result = await _endpoint.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task InvalidContentType_ShouldReturn_UnsupportedMediaType()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";

            // Act
            var result = await _endpoint.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task UnauthorizedApi_ShouldReturn_Unauthorized()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            _apiSecretValidator.Setup(x => x.ValidateAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new ApiSecretValidationResult());

            // Act
            var result = await _endpoint.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ValidRequest_ShouldProcess_Successfully()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var apiResource = new ApiResource("test");
            _apiSecretValidator.Setup(x => x.ValidateAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(new ApiSecretValidationResult { Resource = apiResource });

            var validationResult = new IntrospectionRequestValidationResult 
            { 
                IsActive = true,
                IsError = false,
                Api = apiResource
            };
            _requestValidator.Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ApiResource>()))
                .ReturnsAsync(validationResult);

            var response = new Dictionary<string, object>();
            _responseGenerator.Setup(x => x.ProcessAsync(It.IsAny<IntrospectionRequestValidationResult>()))
                .ReturnsAsync(response);

            // Act
            var result = await _endpoint.ProcessAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<IntrospectionResult>();
            _events.Verify(x => x.RaiseAsync(It.IsAny<Event>()), Times.Never);
        }
    }
}
