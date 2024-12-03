using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServerHost.Extensions.Tests
{
    public class NoSubjectExtensionGrantValidatorTests
    {
        private readonly NoSubjectExtensionGrantValidator _validator;

        public NoSubjectExtensionGrantValidatorTests()
        {
            _validator = new NoSubjectExtensionGrantValidator();
        }

        [Fact]
        public void GrantType_ShouldReturnCorrectValue()
        {
            Assert.Equal("custom.nosubject", _validator.GrantType);
        }

        [Fact]
        public async Task ValidateAsync_WithValidCredential_ShouldSucceed()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection { { "custom_credential", "some_value" } }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            Assert.NotNull(context.Result);
            Assert.True(context.Result.IsError == false);
        }

        [Fact]
        public async Task ValidateAsync_WithMissingCredential_ShouldFail()
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
    }
}
