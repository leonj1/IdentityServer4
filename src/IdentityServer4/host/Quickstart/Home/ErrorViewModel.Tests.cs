using Xunit;
using IdentityServer4.Models;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void DefaultConstructor_CreatesEmptyModel()
        {
            // Arrange & Act
            var model = new ErrorViewModel();

            // Assert
            Assert.Null(model.Error);
        }

        [Fact]
        public void ParameterizedConstructor_SetsErrorMessage()
        {
            // Arrange
            string expectedError = "test_error";

            // Act
            var model = new ErrorViewModel(expectedError);

            // Assert
            Assert.NotNull(model.Error);
            Assert.Equal(expectedError, model.Error.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData("error_code")]
        [InlineData("some_longer_error_message")]
        public void Constructor_WithVariousErrors_SetsErrorCorrectly(string errorMessage)
        {
            // Arrange & Act
            var model = new ErrorViewModel(errorMessage);

            // Assert
            Assert.Equal(errorMessage, model.Error.Error);
        }
    }
}
