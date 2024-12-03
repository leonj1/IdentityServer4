using System;
using Microsoft.AspNetCore.Http;
using Xunit;
using FluentAssertions;
using IdentityServer4.Extensions;
using Microsoft.Net.Http.Headers;

namespace IdentityServer4.UnitTests.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void GetCorsOrigin_WhenOriginHeaderExists_AndDifferentFromHost_ReturnsOrigin()
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.Scheme = "https";
            request.Host = new HostString("localhost:5000");
            request.Headers["Origin"] = "https://example.com";

            // Act
            var result = request.GetCorsOrigin();

            // Assert
            result.Should().Be("https://example.com");
        }

        [Fact]
        public void GetCorsOrigin_WhenOriginHeaderSameAsHost_ReturnsNull()
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.Scheme = "https";
            request.Host = new HostString("localhost:5000");
            request.Headers["Origin"] = "https://localhost:5000";

            // Act
            var result = request.GetCorsOrigin();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetCorsOrigin_WhenOriginHeaderMissing_ReturnsNull()
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.Scheme = "https";
            request.Host = new HostString("localhost:5000");

            // Act
            var result = request.GetCorsOrigin();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void HasApplicationFormContentType_WhenValidContentType_ReturnsTrue()
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.ContentType = "application/x-www-form-urlencoded";

            // Act
            var result = request.HasApplicationFormContentType();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasApplicationFormContentType_WhenValidContentTypeWithCharset_ReturnsTrue()
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

            // Act
            var result = request.HasApplicationFormContentType();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("application/json")]
        [InlineData("text/plain")]
        public void HasApplicationFormContentType_WhenInvalidContentType_ReturnsFalse(string contentType)
        {
            // Arrange
            var request = new DefaultHttpContext().Request;
            request.ContentType = contentType;

            // Act
            var result = request.HasApplicationFormContentType();

            // Assert
            result.Should().BeFalse();
        }
    }
}
