using Xunit;
using IdentityServer4.Models;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class ProcessConsentResultTests
    {
        [Fact]
        public void IsRedirect_WhenRedirectUriIsNull_ReturnsFalse()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                RedirectUri = null
            };

            // Act & Assert
            Assert.False(result.IsRedirect);
        }

        [Fact]
        public void IsRedirect_WhenRedirectUriIsSet_ReturnsTrue()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                RedirectUri = "https://example.com"
            };

            // Act & Assert
            Assert.True(result.IsRedirect);
        }

        [Fact]
        public void ShowView_WhenViewModelIsNull_ReturnsFalse()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                ViewModel = null
            };

            // Act & Assert
            Assert.False(result.ShowView);
        }

        [Fact]
        public void ShowView_WhenViewModelIsSet_ReturnsTrue()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                ViewModel = new ConsentViewModel()
            };

            // Act & Assert
            Assert.True(result.ShowView);
        }

        [Fact]
        public void HasValidationError_WhenValidationErrorIsNull_ReturnsFalse()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                ValidationError = null
            };

            // Act & Assert
            Assert.False(result.HasValidationError);
        }

        [Fact]
        public void HasValidationError_WhenValidationErrorIsSet_ReturnsTrue()
        {
            // Arrange
            var result = new ProcessConsentResult
            {
                ValidationError = "Error message"
            };

            // Act & Assert
            Assert.True(result.HasValidationError);
        }
    }
}
