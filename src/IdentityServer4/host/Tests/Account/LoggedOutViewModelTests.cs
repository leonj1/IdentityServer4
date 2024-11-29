using IdentityServerHost.Quickstart.UI;
using Xunit;

namespace IdentityServer4.Host.Tests.Account
{
    public class LoggedOutViewModelTests
    {
        [Fact]
        public void TriggerExternalSignout_WhenExternalAuthenticationSchemeIsNull_ReturnsFalse()
        {
            // Arrange
            var model = new LoggedOutViewModel
            {
                ExternalAuthenticationScheme = null
            };

            // Act
            var result = model.TriggerExternalSignout;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TriggerExternalSignout_WhenExternalAuthenticationSchemeIsNotNull_ReturnsTrue()
        {
            // Arrange
            var model = new LoggedOutViewModel
            {
                ExternalAuthenticationScheme = "TestScheme"
            };

            // Act
            var result = model.TriggerExternalSignout;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LoggedOutViewModel_PropertiesInitializeToDefaultValues()
        {
            // Arrange & Act
            var model = new LoggedOutViewModel();

            // Assert
            Assert.Null(model.PostLogoutRedirectUri);
            Assert.Null(model.ClientName);
            Assert.Null(model.SignOutIframeUrl);
            Assert.False(model.AutomaticRedirectAfterSignOut);
            Assert.Null(model.LogoutId);
            Assert.Null(model.ExternalAuthenticationScheme);
            Assert.False(model.TriggerExternalSignout);
        }
    }
}
