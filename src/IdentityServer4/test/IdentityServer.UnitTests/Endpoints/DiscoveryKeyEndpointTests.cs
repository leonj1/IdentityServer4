using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints
{
    public class DiscoveryKeyEndpointTests
    {
        private readonly Mock<IDiscoveryResponseGenerator> _mockResponseGenerator;
        private readonly Mock<ILogger<DiscoveryKeyEndpoint>> _mockLogger;
        private readonly IdentityServerOptions _options;
        private readonly DiscoveryKeyEndpoint _endpoint;
        private readonly DefaultHttpContext _context;

        public DiscoveryKeyEndpointTests()
        {
            _mockResponseGenerator = new Mock<IDiscoveryResponseGenerator>();
            _mockLogger = new Mock<ILogger<DiscoveryKeyEndpoint>>();
            _options = new IdentityServerOptions();
            _endpoint = new DiscoveryKeyEndpoint(_options, _mockResponseGenerator.Object, _mockLogger.Object);
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetRequest_WhenKeysEnabled_ReturnsJsonWebKeysResult()
        {
            // Arrange
            _context.Request.Method = "GET";
            var expectedKeys = new[] { new JsonWebKey { Kty = "RSA" } };
            _mockResponseGenerator.Setup(x => x.CreateJwkDocumentAsync())
                .ReturnsAsync(expectedKeys);

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<JsonWebKeysResult>();
            var jsonResult = (JsonWebKeysResult)result;
            jsonResult.Keys.Should().BeEquivalentTo(expectedKeys);
            jsonResult.CacheInterval.Should().Be(_options.Discovery.ResponseCacheInterval);
        }

        [Fact]
        public async Task WhenMethodNotGet_Returns405()
        {
            // Arrange
            _context.Request.Method = "POST";

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task WhenKeysDisabled_Returns404()
        {
            // Arrange
            _context.Request.Method = "GET";
            _options.Discovery.ShowKeySet = false;

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ResponseCacheInterval_IsRespected()
        {
            // Arrange
            _context.Request.Method = "GET";
            _options.Discovery.ResponseCacheInterval = 123;
            var keys = new[] { new JsonWebKey { Kty = "RSA" } };
            _mockResponseGenerator.Setup(x => x.CreateJwkDocumentAsync())
                .ReturnsAsync(keys);

            // Act
            var result = await _endpoint.ProcessAsync(_context);

            // Assert
            result.Should().BeOfType<JsonWebKeysResult>();
            var jsonResult = (JsonWebKeysResult)result;
            jsonResult.CacheInterval.Should().Be(123);
        }
    }
}
