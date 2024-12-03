using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IdentityServer.UnitTests.Validation
{
    public class BearerTokenUsageValidatorTests
    {
        private readonly BearerTokenUsageValidator _validator;
        private readonly ILogger<BearerTokenUsageValidator> _logger;

        public BearerTokenUsageValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<BearerTokenUsageValidator>();
            _validator = new BearerTokenUsageValidator(_logger);
        }

        [Fact]
        public async Task ValidateAsync_WithAuthorizationHeader_ShouldReturnToken()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Bearer testtoken123");

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.TokenFound.Should().BeTrue();
            result.Token.Should().Be("testtoken123");
            result.UsageType.Should().Be(BearerTokenUsageType.AuthorizationHeader);
        }

        [Fact]
        public async Task ValidateAsync_WithPostBody_ShouldReturnToken()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "access_token", "testtoken123" }
            });
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = form;

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.TokenFound.Should().BeTrue();
            result.Token.Should().Be("testtoken123");
            result.UsageType.Should().Be(BearerTokenUsageType.PostBody);
        }

        [Fact]
        public async Task ValidateAsync_WithNoToken_ShouldReturnNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var result = await _validator.ValidateAsync(context);

            // Assert
            result.TokenFound.Should().BeFalse();
            result.Token.Should().BeNull();
        }

        [Fact]
        public void ValidateAuthorizationHeader_WithInvalidScheme_ShouldReturnNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Basic testtoken123");

            // Act
            var result = _validator.ValidateAuthorizationHeader(context);

            // Assert
            result.TokenFound.Should().BeFalse();
            result.Token.Should().BeNull();
        }

        [Fact]
        public async Task ValidatePostBodyAsync_WithNoForm_ShouldReturnNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var result = await _validator.ValidatePostBodyAsync(context);

            // Assert
            result.TokenFound.Should().BeFalse();
            result.Token.Should().BeNull();
        }
    }
}
