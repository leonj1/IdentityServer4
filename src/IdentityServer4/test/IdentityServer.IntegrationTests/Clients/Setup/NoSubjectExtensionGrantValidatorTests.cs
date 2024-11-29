using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class NoSubjectExtensionGrantValidatorTests
    {
        private readonly NoSubjectExtensionGrantValidator _validator;

        public NoSubjectExtensionGrantValidatorTests()
        {
            _validator = new NoSubjectExtensionGrantValidator();
        }

        [Fact]
        public async Task When_Valid_Credential_Provided_Should_Succeed()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "custom_credential", "some_value" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task When_No_Credential_Provided_Should_Fail()
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
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(TokenRequestErrors.InvalidGrant);
            context.Result.ErrorDescription.Should().Be("invalid custom credential");
        }

        [Fact]
        public void GrantType_Should_Return_Expected_Value()
        {
            // Assert
            _validator.GrantType.Should().Be("custom.nosubject");
        }
    }
}
