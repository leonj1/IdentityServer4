using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class CustomTokenValidatorTests
    {
        private class MockCustomTokenValidator : ICustomTokenValidator
        {
            public bool AccessTokenCalled { get; private set; }
            public bool IdentityTokenCalled { get; private set; }

            public Task<TokenValidationResult> ValidateAccessTokenAsync(TokenValidationResult result)
            {
                AccessTokenCalled = true;
                result.IsError = false;
                return Task.FromResult(result);
            }

            public Task<TokenValidationResult> ValidateIdentityTokenAsync(TokenValidationResult result)
            {
                IdentityTokenCalled = true;
                result.IsError = false;
                return Task.FromResult(result);
            }
        }

        [Fact]
        public async Task ValidateAccessToken_Should_Execute_Custom_Validation()
        {
            // Arrange
            var validator = new MockCustomTokenValidator();
            var result = new TokenValidationResult();

            // Act
            var validationResult = await validator.ValidateAccessTokenAsync(result);

            // Assert
            validator.AccessTokenCalled.Should().BeTrue();
            validationResult.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateIdentityToken_Should_Execute_Custom_Validation()
        {
            // Arrange
            var validator = new MockCustomTokenValidator();
            var result = new TokenValidationResult();

            // Act
            var validationResult = await validator.ValidateIdentityTokenAsync(result);

            // Assert
            validator.IdentityTokenCalled.Should().BeTrue();
            validationResult.IsError.Should().BeFalse();
        }
    }
}
