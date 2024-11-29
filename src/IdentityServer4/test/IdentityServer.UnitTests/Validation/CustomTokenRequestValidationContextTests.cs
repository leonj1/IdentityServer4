using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class CustomTokenRequestValidationContextTests
    {
        [Fact]
        public void Result_Should_Be_Settable()
        {
            // Arrange
            var context = new CustomTokenRequestValidationContext();
            var result = new TokenRequestValidationResult(new TokenRequestValidationResult());

            // Act
            context.Result = result;

            // Assert
            context.Result.Should().BeSameAs(result);
        }

        [Fact]
        public void Result_Should_Default_To_Null()
        {
            // Arrange
            var context = new CustomTokenRequestValidationContext();

            // Assert
            context.Result.Should().BeNull();
        }
    }
}
