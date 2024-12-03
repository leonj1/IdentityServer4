using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestTokenValidatorTests
    {
        [Fact]
        public async Task ValidateAccessToken_ShouldReturnConfiguredResult()
        {
            // Arrange
            var expectedResult = new TokenValidationResult { IsError = false };
            var validator = new TestTokenValidator(expectedResult);

            // Act
            var result = await validator.ValidateAccessTokenAsync("test_token", "test_scope");

            // Assert
            result.Should().BeSameAs(expectedResult);
        }

        [Fact]
        public async Task ValidateIdentityToken_ShouldReturnConfiguredResult()
        {
            // Arrange
            var expectedResult = new TokenValidationResult { IsError = false };
            var validator = new TestTokenValidator(expectedResult);

            // Act
            var result = await validator.ValidateIdentityTokenAsync("test_token", "test_client", true);

            // Assert
            result.Should().BeSameAs(expectedResult);
        }

        [Fact]
        public async Task ValidateRefreshToken_ShouldReturnConfiguredResult()
        {
            // Arrange
            var expectedResult = new TokenValidationResult { IsError = false };
            var validator = new TestTokenValidator(expectedResult);
            var client = new Client();

            // Act
            var result = await validator.ValidateRefreshTokenAsync("test_token", client);

            // Assert
            result.Should().BeSameAs(expectedResult);
        }
    }
}
