using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer4.AspNetIdentity.UnitTests.Quickstart.Account
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
        public void RedirectUrl_ShouldAllowNull()
        {
            // Arrange
            var model = new RedirectViewModel();

            // Act & Assert
            Assert.Null(model.RedirectUrl);
        }
    }
}
