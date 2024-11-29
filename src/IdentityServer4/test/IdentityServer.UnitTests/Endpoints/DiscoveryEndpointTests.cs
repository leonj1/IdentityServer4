using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class DiscoveryEndpointTests
    {
        private readonly Mock<IDiscoveryResponseGenerator> _mockResponseGenerator;
        private readonly Mock<ILogger<DiscoveryEndpoint>> _mockLogger;
        private readonly IdentityServerOptions _options;
        private readonly DiscoveryEndpoint _endpoint;
        private readonly DefaultHttpContext _context;

        public DiscoveryEndpointTests()
        {
            _mockResponseGenerator = new Mock<IDiscoveryResponseGenerator>();
            _mockLogger = new Mock<ILogger<DiscoveryEndpoint>>();
            _options = new IdentityServerOptions();
            _endpoint = new DiscoveryEndpoint(_options, _mockResponseGenerator.Object, _mockLogger.Object);
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task ProcessAsync_WhenMethodNotGet_ReturnsMethodNotAllowed()
        {
            // Arrange
            _context.Request.Method = "POST";

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = (StatusCodeResult)result;
            statusResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task ProcessAsync_WhenEndpointDisabled_ReturnsNotFound()
        {
            // Arrange
            _context.Request.Method = "GET";
            _options.Endpoints.EnableDiscoveryEndpoint = false;

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = (StatusCodeResult)result;
            statusResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ProcessAsync_WhenValidRequest_ReturnsDiscoveryDocument()
        {
            // Arrange
            _context.Request.Method = "GET";
            _options.Endpoints.EnableDiscoveryEndpoint = true;
            
            var discoveryDocument = new System.Collections.Generic.Dictionary<string, object>();
            _mockResponseGenerator
                .Setup(x => x.CreateDiscoveryDocumentAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(discoveryDocument);

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<DiscoveryDocumentResult>();
            var documentResult = (DiscoveryDocumentResult)result;
            documentResult.Entries.Should().BeSameAs(discoveryDocument);
            documentResult.MaxAge.Should().Be(_options.Discovery.ResponseCacheInterval);
        }

        [Theory]
        [InlineData("/.well-known/openid-configuration")]
        [InlineData("/.well-known/openid-configuration/")]
        [InlineData("/")]
        public async Task ProcessAsync_WithValidPaths_CallsResponseGenerator(string path)
        {
            // Arrange
            _context.Request.Method = "GET";
            _context.Request.Path = path;
            var discoveryDocument = new System.Collections.Generic.Dictionary<string, object>();
            _mockResponseGenerator
                .Setup(x => x.CreateDiscoveryDocumentAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(discoveryDocument);

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<DiscoveryDocumentResult>();
            _mockResponseGenerator.Verify(x => x.CreateDiscoveryDocumentAsync(
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WhenJwksRequest_ReturnsJwkDocument()
        {
            // Arrange
            _context.Request.Method = "GET";
            _context.Request.Path = "/.well-known/openid-configuration/jwks";
            var jwks = new System.Collections.Generic.Dictionary<string, object>();
            _mockResponseGenerator
                .Setup(x => x.CreateJwkDocumentAsync())
                .ReturnsAsync(jwks);

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<JsonWebKeysResult>();
            var jwksResult = (JsonWebKeysResult)result;
            jwksResult.Entries.Should().BeSameAs(jwks);
        }

        [Fact]
        public async Task ProcessAsync_WithInvalidPath_ReturnsNotFound()
        {
            // Arrange
            _context.Request.Method = "GET";
            _context.Request.Path = "/invalid-path";

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = (StatusCodeResult)result;
            statusResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
