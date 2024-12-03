using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class EndSessionEndpointTests
    {
        private readonly Mock<IEndSessionRequestValidator> _mockValidator;
        private readonly Mock<IUserSession> _mockUserSession;
        private readonly Mock<ILogger<EndSessionEndpoint>> _mockLogger;
        private readonly EndSessionEndpoint _subject;

        public EndSessionEndpointTests()
        {
            _mockValidator = new Mock<IEndSessionRequestValidator>();
            _mockUserSession = new Mock<IUserSession>();
            _mockLogger = new Mock<ILogger<EndSessionEndpoint>>();
            
            _subject = new EndSessionEndpoint(
                _mockValidator.Object,
                _mockUserSession.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetRequest_ValidatesRequest()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            ctx.Request.QueryString = new QueryString("?id_token_hint=token");

            var validationResult = new EndSessionValidationResult();
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _subject.ProcessAsync(ctx);

            // Assert
            result.Should().BeOfType<EndSessionResult>();
            var endSessionResult = (EndSessionResult)result;
            endSessionResult.ValidationResult.Should().Be(validationResult);
        }

        [Fact]
        public async Task PostRequest_ValidatesRequest()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";
            ctx.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>
                {
                    { "id_token_hint", "token" }
                });

            var validationResult = new EndSessionValidationResult();
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _subject.ProcessAsync(ctx);

            // Assert
            result.Should().BeOfType<EndSessionResult>();
            var endSessionResult = (EndSessionResult)result;
            endSessionResult.ValidationResult.Should().Be(validationResult);
        }

        [Fact]
        public async Task InvalidMethod_ReturnsMethodNotAllowed()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "PUT";

            // Act
            var result = await _subject.ProcessAsync(ctx);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult)result;
            statusCodeResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task ValidationError_LogsError()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            
            var validationResult = new EndSessionValidationResult { 
                IsError = true,
                Error = "test_error"
            };
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _subject.ProcessAsync(ctx);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
