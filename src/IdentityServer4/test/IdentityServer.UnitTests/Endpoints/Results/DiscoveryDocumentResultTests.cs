using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class DiscoveryDocumentResultTests
    {
        [Fact]
        public void Constructor_WithNullEntries_ThrowsArgumentNullException()
        {
            Action act = () => new DiscoveryDocumentResult(null, 60);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("entries");
        }

        [Fact]
        public async Task ExecuteAsync_WithValidMaxAge_SetsCacheHeaders()
        {
            // Arrange
            var entries = new Dictionary<string, object>
            {
                { "issuer", "https://demo" },
                { "authorization_endpoint", "https://demo/connect/authorize" }
            };
            var result = new DiscoveryDocumentResult(entries, 60);

            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.Headers["Cache-Control"].ToString()
                .Should().Contain("max-age=60");
            context.Response.Headers["Vary"].ToString()
                .Should().Contain("Origin");
        }

        [Fact]
        public async Task ExecuteAsync_WithNegativeMaxAge_DoesNotSetCache()
        {
            // Arrange
            var entries = new Dictionary<string, object>
            {
                { "issuer", "https://demo" }
            };
            var result = new DiscoveryDocumentResult(entries, -1);

            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.Headers.Should().NotContainKey("Cache-Control");
            context.Response.Headers.Should().NotContainKey("Vary");
        }

        [Fact]
        public async Task ExecuteAsync_WritesEntriesAsJson()
        {
            // Arrange
            var entries = new Dictionary<string, object>
            {
                { "issuer", "https://demo" },
                { "jwks_uri", "https://demo/.well-known/jwks" }
            };
            var result = new DiscoveryDocumentResult(entries, null);

            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();

            // Act
            await result.ExecuteAsync(context);

            // Assert
            context.Response.ContentType.Should().Be("application/json; charset=UTF-8");
            context.Response.StatusCode.Should().Be(200);
        }
    }
}
