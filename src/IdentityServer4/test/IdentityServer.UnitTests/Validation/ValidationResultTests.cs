using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ValidationResultTests
    {
        [Fact]
        public void Default_Constructor_Should_Set_IsError_To_True()
        {
            // Act
            var result = new ValidationResult();

            // Assert
            result.IsError.Should().BeTrue();
        }

        [Fact]
        public void Can_Set_Error_Properties()
        {
            // Arrange
            var result = new ValidationResult
            {
                Error = "test_error",
                ErrorDescription = "test error description",
                IsError = false
            };

            // Assert
            result.Error.Should().Be("test_error");
            result.ErrorDescription.Should().Be("test error description");
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void Properties_Should_Default_To_Null()
        {
            // Act
            var result = new ValidationResult();

            // Assert
            result.Error.Should().BeNull();
            result.ErrorDescription.Should().BeNull();
        }
    }
}
