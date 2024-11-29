using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServerHost.Extensions.Tests
{
    public class ExtensionGrantValidatorTests
    {
        private readonly ExtensionGrantValidator _validator;

        public ExtensionGrantValidatorTests()
        {
            _validator = new ExtensionGrantValidator();
        }

        [Fact]
        public async Task ValidateAsync_WithValidCredential_ShouldReturnSuccessResult()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection { { "custom_credential", "any_value" } }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError == false);
            Assert.Equal("818727", context.Result.Subject);
            Assert.Equal("custom", context.Result.AuthenticationMethod);
        }

        [Fact]
        public async Task ValidateAsync_WithMissingCredential_ShouldReturnError()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection()
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError);
            Assert.Equal(TokenRequestErrors.InvalidGrant, context.Result.Error);
            Assert.Equal("invalid custom credential", context.Result.ErrorDescription);
        }

        [Fact]
        public void GrantType_ShouldReturnCustom()
        {
            // Assert
            Assert.Equal("custom", _validator.GrantType);
        }
    }
}
