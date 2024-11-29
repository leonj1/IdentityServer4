using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
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
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "custom_credential", "any_value" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("818727");
            context.Result.CustomResponse.Should().BeNull();
        }

        [Fact]
        public async Task ValidateAsync_WithValidCredentialAndExtraClaim_ShouldReturnSuccessResultWithClaim()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "custom_credential", "any_value" },
                        { "extra_claim", "claim_value" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("818727");
            context.Result.Claims.Should().NotBeEmpty();
            context.Result.Claims.First().Type.Should().Be("extra_claim");
            context.Result.Claims.First().Value.Should().Be("claim_value");
        }

        [Fact]
        public async Task ValidateAsync_WithInvalidCredential_ShouldReturnErrorResult()
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
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be("invalid_grant");
            context.Result.ErrorDescription.Should().Be("invalid_custom_credential");
        }

        [Fact]
        public void GrantType_ShouldReturnCustom()
        {
            // Act & Assert
            _validator.GrantType.Should().Be("custom");
        }
    }
}
