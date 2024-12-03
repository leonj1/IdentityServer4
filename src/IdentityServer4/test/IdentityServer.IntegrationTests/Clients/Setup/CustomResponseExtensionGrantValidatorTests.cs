using System.Threading.Tasks;
using IdentityServer4.Validation;
using Xunit;
using FluentAssertions;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class CustomResponseExtensionGrantValidatorTests
    {
        private readonly CustomResponseExtensionGrantValidator _validator;

        public CustomResponseExtensionGrantValidatorTests()
        {
            _validator = new CustomResponseExtensionGrantValidator();
        }

        [Fact]
        public void GrantType_ShouldReturn_Custom()
        {
            _validator.GrantType.Should().Be("custom");
        }

        [Fact]
        public async Task ValidateAsync_WhenOutcomeIsSucceed_ShouldReturnSuccessResult()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "outcome", "succeed" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("bob");
            context.Result.CustomResponse.Should().ContainKey("string_value");
            context.Result.CustomResponse["string_value"].Should().Be("some_string");
            context.Result.CustomResponse.Should().ContainKey("int_value");
            context.Result.CustomResponse["int_value"].Should().Be(42);
            context.Result.CustomResponse.Should().ContainKey("dto");
        }

        [Fact]
        public async Task ValidateAsync_WhenOutcomeIsNotSucceed_ShouldReturnErrorResult()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new System.Collections.Specialized.NameValueCollection
                    {
                        { "outcome", "fail" }
                    }
                }
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be("invalid_grant");
            context.Result.ErrorDescription.Should().Be("invalid_credential");
            context.Result.CustomResponse.Should().ContainKey("string_value");
            context.Result.CustomResponse["string_value"].Should().Be("some_string");
            context.Result.CustomResponse.Should().ContainKey("int_value");
            context.Result.CustomResponse["int_value"].Should().Be(42);
            context.Result.CustomResponse.Should().ContainKey("dto");
        }
    }
}
