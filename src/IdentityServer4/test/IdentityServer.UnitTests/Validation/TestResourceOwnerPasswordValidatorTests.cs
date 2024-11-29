using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TestResourceOwnerPasswordValidatorTests
    {
        [Fact]
        public async Task When_Credentials_Match_Should_Return_Success()
        {
            // Arrange
            var validator = new TestResourceOwnerPasswordValidator();
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "testuser"
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("testuser");
        }

        [Fact]
        public async Task When_Credentials_Dont_Match_Should_Return_Null_Result()
        {
            // Arrange
            var validator = new TestResourceOwnerPasswordValidator();
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "wrong"
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public async Task When_Empty_Password_Allowed_Should_Return_Success()
        {
            // Arrange
            var validator = new TestResourceOwnerPasswordValidator();
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "bob_no_password",
                Password = ""
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("bob_no_password");
        }

        [Fact]
        public async Task When_Error_Configured_Should_Return_Error()
        {
            // Arrange
            var expectedError = TokenRequestErrors.InvalidGrant;
            var expectedErrorDescription = "test error";
            var validator = new TestResourceOwnerPasswordValidator(expectedError, expectedErrorDescription);
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "testpass"
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(expectedError);
            context.Result.ErrorDescription.Should().Be(expectedErrorDescription);
        }
    }
}
