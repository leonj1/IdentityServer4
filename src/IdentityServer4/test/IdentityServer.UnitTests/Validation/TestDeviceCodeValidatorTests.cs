using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using IdentityServer.UnitTests.Validation.Setup;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TestDeviceCodeValidatorTests
    {
        [Fact]
        public async Task When_Validation_Succeeds_Result_Should_Not_Have_Error()
        {
            // Arrange
            var validator = new TestDeviceCodeValidator();
            var context = new DeviceCodeValidationContext { Request = new ValidatedTokenRequest() };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task When_ShouldError_Is_True_Result_Should_Have_Error()
        {
            // Arrange
            var validator = new TestDeviceCodeValidator(shouldError: true);
            var context = new DeviceCodeValidationContext { Request = new ValidatedTokenRequest() };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be("error");
        }
    }
}
