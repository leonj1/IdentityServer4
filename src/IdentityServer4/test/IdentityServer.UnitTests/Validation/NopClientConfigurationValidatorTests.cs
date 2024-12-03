using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class NopClientConfigurationValidatorTests
    {
        private readonly NopClientConfigurationValidator _validator;

        public NopClientConfigurationValidatorTests()
        {
            _validator = new NopClientConfigurationValidator();
        }

        [Fact]
        public async Task ValidateAsync_ShouldAlwaysReturnValid()
        {
            // Arrange
            var context = new ClientConfigurationValidationContext(new Client());

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.IsValid.Should().BeTrue();
        }
    }
}
