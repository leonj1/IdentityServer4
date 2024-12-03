using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class RedirectViewModelTests
    {
        [Fact]
        public void RedirectUrl_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var model = new RedirectViewModel();
            var expectedUrl = "https://example.com";

            // Act
            model.RedirectUrl = expectedUrl;

            // Assert
            Assert.Equal(expectedUrl, model.RedirectUrl);
        }

        [Fact]
        public void RedirectUrl_ShouldDefaultToNull()
        {
            // Arrange & Act
            var model = new RedirectViewModel();

            // Assert
            Assert.Null(model.RedirectUrl);
        }
    }
}
