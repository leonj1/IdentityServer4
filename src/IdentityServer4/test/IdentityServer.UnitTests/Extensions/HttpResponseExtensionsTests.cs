using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class HttpResponseExtensionsTests
    {
        private readonly DefaultHttpContext _context;
        private readonly HttpResponse _response;

        public HttpResponseExtensionsTests()
        {
            _context = new DefaultHttpContext();
            _response = _context.Response;
        }

        [Fact]
        public void SetCache_WhenMaxAgeIsZero_ShouldSetNoCacheHeaders()
        {
            // Act
            _response.SetCache(0);

            // Assert
            _response.Headers["Cache-Control"].ToString().Should().Be("no-store, no-cache, max-age=0");
            _response.Headers["Pragma"].ToString().Should().Be("no-cache");
        }

        [Fact]
        public void SetCache_WhenMaxAgeIsPositive_ShouldSetMaxAgeHeader()
        {
            // Act
            _response.SetCache(100);

            // Assert
            _response.Headers["Cache-Control"].ToString().Should().Be("max-age=100");
        }

        [Fact]
        public void SetCache_WithVaryBy_ShouldSetVaryHeader()
        {
            // Act
            _response.SetCache(100, "Accept", "Accept-Language");

            // Assert
            _response.Headers["Cache-Control"].ToString().Should().Be("max-age=100");
            _response.Headers["Vary"].ToString().Should().Be("Accept,Accept-Language");
        }

        [Fact]
        public async Task WriteJsonAsync_ShouldSetContentTypeAndWriteJson()
        {
            // Arrange
            var testObject = new { name = "test" };

            // Act
            await _response.WriteJsonAsync(testObject);

            // Assert
            _response.ContentType.Should().Be("application/json; charset=UTF-8");
        }

        [Fact]
        public void AddScriptCspHeaders_ShouldSetContentSecurityPolicyHeaders()
        {
            // Arrange
            var options = new CspOptions { Level = CspLevel.Two };
            var hash = "sha256-123456";

            // Act
            _response.AddScriptCspHeaders(options, hash);

            // Assert
            _response.Headers["Content-Security-Policy"].ToString()
                .Should().Be($"default-src 'none'; script-src '{hash}'");
        }

        [Fact]
        public void AddStyleCspHeaders_ShouldSetContentSecurityPolicyHeaders()
        {
            // Arrange
            var options = new CspOptions { Level = CspLevel.Two };
            var hash = "sha256-123456";
            var frameSources = "https://example.com";

            // Act
            _response.AddStyleCspHeaders(options, hash, frameSources);

            // Assert
            _response.Headers["Content-Security-Policy"].ToString()
                .Should().Be($"default-src 'none'; style-src '{hash}'; frame-src {frameSources}");
        }
    }
}
