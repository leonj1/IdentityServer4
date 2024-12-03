using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class DefaultCustomAuthorizeRequestValidatorTests
    {
        private readonly DefaultCustomAuthorizeRequestValidator _validator;

        public DefaultCustomAuthorizeRequestValidatorTests()
        {
            _validator = new DefaultCustomAuthorizeRequestValidator();
        }

        [Fact]
        public async Task ValidateAsync_ShouldComplete_WithoutModifyingContext()
        {
            // Arrange
            var context = new CustomAuthorizeRequestValidationContext();

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.Should().BeNull();
        }
    }
}
