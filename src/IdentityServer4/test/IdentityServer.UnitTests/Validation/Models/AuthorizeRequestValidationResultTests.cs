using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class AuthorizeRequestValidationResultTests
    {
        [Fact]
        public void Constructor_WithOnlyRequest_SetsPropertiesCorrectly()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest();

            // Act
            var result = new AuthorizeRequestValidationResult(request);

            // Assert
            result.ValidatedRequest.Should().BeSameAs(request);
            result.IsError.Should().BeFalse();
            result.Error.Should().BeNull();
            result.ErrorDescription.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithErrorDetails_SetsPropertiesCorrectly()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest();
            var error = "test_error";
            var errorDescription = "test error description";

            // Act
            var result = new AuthorizeRequestValidationResult(request, error, errorDescription);

            // Assert
            result.ValidatedRequest.Should().BeSameAs(request);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(error);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        [Fact]
        public void Constructor_WithErrorOnly_SetsPropertiesCorrectly()
        {
            // Arrange
            var request = new ValidatedAuthorizeRequest();
            var error = "test_error";

            // Act
            var result = new AuthorizeRequestValidationResult(request, error);

            // Assert
            result.ValidatedRequest.Should().BeSameAs(request);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(error);
            result.ErrorDescription.Should().BeNull();
        }
    }
}
