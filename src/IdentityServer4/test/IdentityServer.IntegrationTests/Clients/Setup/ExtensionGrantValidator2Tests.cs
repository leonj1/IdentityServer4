using System.Threading.Tasks;
using IdentityServer4.Validation;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class ExtensionGrantValidator2Tests
    {
        private readonly ExtensionGrantValidator2 _validator;

        public ExtensionGrantValidator2Tests()
        {
            _validator = new ExtensionGrantValidator2();
        }

        [Fact]
        public async Task ValidateAsync_WithValidCredential_ShouldReturnSuccessResult()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "custom_credential", "any_value" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsValid);
            Assert.Equal("818727", context.Result.Subject);
            Assert.Equal("custom", context.Result.CustomResponse);
        }

        [Fact]
        public async Task ValidateAsync_WithInvalidCredential_ShouldReturnFailureResult()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection()
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.False(context.Result.IsValid);
            Assert.Equal(TokenRequestErrors.InvalidGrant, context.Result.Error);
            Assert.Equal("invalid custom credential", context.Result.ErrorDescription);
        }

        [Fact]
        public void GrantType_ShouldReturnCorrectValue()
        {
            // Assert
            Assert.Equal("custom2", _validator.GrantType);
        }
    }
}
