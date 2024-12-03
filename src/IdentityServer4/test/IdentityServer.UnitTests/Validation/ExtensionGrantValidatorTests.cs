using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ExtensionGrantValidatorTests
    {
        private class TestExtensionGrantValidator : IExtensionGrantValidator
        {
            public string GrantType => "custom_grant";

            public Task ValidateAsync(ExtensionGrantValidationContext context)
            {
                context.Result = new GrantValidationResult("subject_id");
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task ValidateAsync_Should_SetResultWithSubjectId()
        {
            // Arrange
            var validator = new TestExtensionGrantValidator();
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest()
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.Subject.FindFirst("sub").Value.Should().Be("subject_id");
        }

        [Fact]
        public void GrantType_Should_ReturnExpectedValue()
        {
            // Arrange
            var validator = new TestExtensionGrantValidator();

            // Act & Assert
            validator.GrantType.Should().Be("custom_grant");
        }
    }
}
