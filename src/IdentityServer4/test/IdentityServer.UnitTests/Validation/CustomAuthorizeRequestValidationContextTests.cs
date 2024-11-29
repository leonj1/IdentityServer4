using System;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class CustomAuthorizeRequestValidationContextTests
    {
        [Fact]
        public void Result_Should_Be_Settable()
        {
            // Arrange
            var context = new CustomAuthorizeRequestValidationContext();
            var result = new AuthorizeRequestValidationResult();

            // Act
            context.Result = result;

            // Assert
            context.Result.Should().BeSameAs(result);
        }

        [Fact]
        public void Result_Should_Allow_Null()
        {
            // Arrange
            var context = new CustomAuthorizeRequestValidationContext();

            // Act
            context.Result = null;

            // Assert
            context.Result.Should().BeNull();
        }
    }
}
