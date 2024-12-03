using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class LoggedOutViewModelTests
    {
        [Fact]
        public void TriggerExternalSignout_WhenExternalAuthenticationSchemeIsNull_ReturnsFalse()
        {
            // Arrange
            var viewModel = new LoggedOutViewModel
            {
                ExternalAuthenticationScheme = null
            };

            // Act
            bool result = viewModel.TriggerExternalSignout;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TriggerExternalSignout_WhenExternalAuthenticationSchemeIsNotNull_ReturnsTrue()
        {
            // Arrange
            var viewModel = new LoggedOutViewModel
            {
                ExternalAuthenticationScheme = "TestScheme"
            };

            // Act
            bool result = viewModel.TriggerExternalSignout;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LoggedOutViewModel_PropertiesInitializeToDefaultValues()
        {
            // Arrange & Act
            var viewModel = new LoggedOutViewModel();

            // Assert
            Assert.Null(viewModel.PostLogoutRedirectUri);
            Assert.Null(viewModel.ClientName);
            Assert.Null(viewModel.SignOutIframeUrl);
            Assert.False(viewModel.AutomaticRedirectAfterSignOut);
            Assert.Null(viewModel.LogoutId);
            Assert.Null(viewModel.ExternalAuthenticationScheme);
            Assert.False(viewModel.TriggerExternalSignout);
        }
    }
}
