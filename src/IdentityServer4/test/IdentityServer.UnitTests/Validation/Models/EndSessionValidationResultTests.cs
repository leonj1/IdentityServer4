using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class EndSessionValidationResultTests
    {
        [Fact]
        public void Should_Inherit_From_ValidationResult()
        {
            // Arrange
            var result = new EndSessionValidationResult();

            // Assert
            result.Should().BeAssignableTo<ValidationResult>();
        }

        [Fact]
        public void Should_Have_ValidatedRequest_Property()
        {
            // Arrange
            var result = new EndSessionValidationResult();
            var request = new ValidatedEndSessionRequest();

            // Act
            result.ValidatedRequest = request;

            // Assert
            result.ValidatedRequest.Should().Be(request);
        }

        [Fact]
        public void Should_Inherit_IsError_From_Base()
        {
            // Arrange
            var result = new EndSessionValidationResult();

            // Act
            result.IsError = true;

            // Assert
            result.IsError.Should().BeTrue();
        }
    }
}
