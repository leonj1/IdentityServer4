using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class CustomAuthorizeRequestValidatorTests
    {
        private class TestValidator : ICustomAuthorizeRequestValidator
        {
            public bool WasCalled { get; private set; }
            
            public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
            {
                WasCalled = true;
                context.Result = new AuthorizeRequestValidationResult(context.Result.ValidatedRequest);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Validator_Should_Be_Called_And_Process_Context()
        {
            // Arrange
            var validator = new TestValidator();
            var context = new CustomAuthorizeRequestValidationContext(
                new AuthorizeRequestValidationResult(new ValidatedAuthorizeRequest()));

            // Act
            await validator.ValidateAsync(context);

            // Assert
            validator.WasCalled.Should().BeTrue();
            context.Result.Should().NotBeNull();
        }
    }
}
