using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ExtensionGrantValidationContextTests
    {
        [Fact]
        public void Constructor_Should_SetDefaultResult()
        {
            // Arrange & Act
            var context = new ExtensionGrantValidationContext();

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.Error.Should().Be(TokenRequestErrors.InvalidGrant);
        }

        [Fact]
        public void Properties_Should_AllowSettingAndGetting()
        {
            // Arrange
            var context = new ExtensionGrantValidationContext();
            var request = new ValidatedTokenRequest();
            var result = new GrantValidationResult(TokenRequestErrors.InvalidClient);

            // Act
            context.Request = request;
            context.Result = result;

            // Assert
            context.Request.Should().BeSameAs(request);
            context.Result.Should().BeSameAs(result);
        }
    }
}
