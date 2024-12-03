using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.EndSessionRequestValidation
{
    public class StubTokenValidatorTests
    {
        private StubTokenValidator _validator;

        public StubTokenValidatorTests()
        {
            _validator = new StubTokenValidator();
        }

        [Fact]
        public async Task ValidateAccessToken_ShouldReturnDefaultResult_WhenNoCustomResultSet()
        {
            // Act
            var result = await _validator.ValidateAccessTokenAsync("sometoken");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TokenValidationResult>();
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAccessToken_ShouldReturnCustomResult_WhenCustomResultSet()
        {
            // Arrange
            var customResult = new TokenValidationResult
            {
                IsError = true,
                Error = "custom_error"
            };
            _validator.AccessTokenValidationResult = customResult;

            // Act
            var result = await _validator.ValidateAccessTokenAsync("sometoken");

            // Assert
            result.Should().Be(customResult);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("custom_error");
        }

        [Fact]
        public async Task ValidateIdentityToken_ShouldReturnDefaultResult_WhenNoCustomResultSet()
        {
            // Act
            var result = await _validator.ValidateIdentityTokenAsync("sometoken");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TokenValidationResult>();
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateIdentityToken_ShouldReturnCustomResult_WhenCustomResultSet()
        {
            // Arrange
            var customResult = new TokenValidationResult
            {
                IsError = true,
                Error = "custom_error"
            };
            _validator.IdentityTokenValidationResult = customResult;

            // Act
            var result = await _validator.ValidateIdentityTokenAsync("sometoken");

            // Assert
            result.Should().Be(customResult);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("custom_error");
        }

        [Fact]
        public async Task ValidateRefreshToken_ShouldThrowNotImplementedException()
        {
            // Arrange
            var client = new Client();

            // Act & Assert
            await Assert.ThrowsAsync<System.NotImplementedException>(
                () => _validator.ValidateRefreshTokenAsync("sometoken", client));
        }
    }
}
