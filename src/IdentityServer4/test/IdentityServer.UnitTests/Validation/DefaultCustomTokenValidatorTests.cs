using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class DefaultCustomTokenValidatorTests
    {
        private readonly DefaultCustomTokenValidator _validator;

        public DefaultCustomTokenValidatorTests()
        {
            _validator = new DefaultCustomTokenValidator();
        }

        [Fact]
        public async Task ValidateAccessToken_ShouldReturnSameResult()
        {
            // Arrange
            var result = new TokenValidationResult
            {
                IsError = false,
                Claims = new System.Security.Claims.ClaimsPrincipal()
            };

            // Act
            var validationResult = await _validator.ValidateAccessTokenAsync(result);

            // Assert
            validationResult.Should().BeSameAs(result);
            validationResult.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateIdentityToken_ShouldReturnSameResult()
        {
            // Arrange
            var result = new TokenValidationResult
            {
                IsError = false,
                Claims = new System.Security.Claims.ClaimsPrincipal()
            };

            // Act
            var validationResult = await _validator.ValidateIdentityTokenAsync(result);

            // Assert
            validationResult.Should().BeSameAs(result);
            validationResult.IsError.Should().BeFalse();
        }
    }
}
