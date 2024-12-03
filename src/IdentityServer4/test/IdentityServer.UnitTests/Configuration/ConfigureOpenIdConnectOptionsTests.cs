using System;
using FluentAssertions;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;
using IdentityServer4.Infrastructure;

namespace IdentityServer.UnitTests.Configuration
{
    public class ConfigureOpenIdConnectOptionsTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _schemes;
        private readonly OpenIdConnectOptions _options;

        public ConfigureOpenIdConnectOptionsTests()
        {
            _httpContextAccessor = new HttpContextAccessor();
            _schemes = new[] { "test-scheme" };
            _options = new OpenIdConnectOptions();
        }

        [Fact]
        public void Constructor_WhenSchemesIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ConfigureOpenIdConnectOptions(null, _httpContextAccessor);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("schemes");
        }

        [Fact]
        public void Constructor_WhenHttpContextAccessorIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => new ConfigureOpenIdConnectOptions(_schemes, null);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("httpContextAccessor");
        }

        [Fact]
        public void PostConfigure_WhenSchemeMatches_SetsStateDataFormat()
        {
            // Arrange
            var sut = new ConfigureOpenIdConnectOptions(_schemes, _httpContextAccessor);

            // Act
            sut.PostConfigure("test-scheme", _options);

            // Assert
            _options.StateDataFormat.Should().BeOfType<DistributedCacheStateDataFormatter>();
        }

        [Fact]
        public void PostConfigure_WhenEmptySchemes_ConfiguresAllSchemes()
        {
            // Arrange
            var emptySchemes = Array.Empty<string>();
            var sut = new ConfigureOpenIdConnectOptions(emptySchemes, _httpContextAccessor);

            // Act
            sut.PostConfigure("any-scheme", _options);

            // Assert
            _options.StateDataFormat.Should().BeOfType<DistributedCacheStateDataFormatter>();
        }

        [Fact]
        public void PostConfigure_WhenSchemeDoesNotMatch_DoesNotSetStateDataFormat()
        {
            // Arrange
            var sut = new ConfigureOpenIdConnectOptions(_schemes, _httpContextAccessor);
            var originalStateDataFormat = _options.StateDataFormat;

            // Act
            sut.PostConfigure("different-scheme", _options);

            // Assert
            _options.StateDataFormat.Should().Be(originalStateDataFormat);
        }
    }
}
