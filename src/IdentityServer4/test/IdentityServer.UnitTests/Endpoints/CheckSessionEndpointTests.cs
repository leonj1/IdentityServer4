using FluentAssertions;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class CheckSessionEndpointTests
    {
        private readonly Mock<ILogger<CheckSessionEndpoint>> _logger;
        private readonly CheckSessionEndpoint _subject;

        public CheckSessionEndpointTests()
        {
            _logger = new Mock<ILogger<CheckSessionEndpoint>>();
            _subject = new CheckSessionEndpoint(_logger.Object);
        }

        [Fact]
        public async Task GetRequest_ReturnsCheckSessionResult()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<CheckSessionResult>();
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task InvalidHttpMethod_ReturnsMethodNotAllowed(string method)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = method;

            // Act
            var result = await _subject.ProcessAsync(context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = (StatusCodeResult)result;
            statusResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }
    }
}
