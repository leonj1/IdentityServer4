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

        [Fact]
        public void ErrorProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var model = new ErrorViewModel();
            var errorMessage = new ErrorMessage { Error = "test_error" };

            // Act
            model.Error = errorMessage;

            // Assert
            Assert.Same(errorMessage, model.Error);
            Assert.Equal("test_error", model.Error.Error);
        }
    }
}
