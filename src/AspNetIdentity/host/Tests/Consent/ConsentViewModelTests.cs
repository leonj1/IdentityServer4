using System.Collections.Generic;
using System.Linq;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Consent
{
    public class ConsentViewModelTests
    {
        [Fact]
        public void ConsentViewModel_ShouldInitializeCorrectly()
        {
            // Arrange
            var model = new ConsentViewModel
            {
                ClientName = "Test Client",
                ClientUrl = "https://test.com",
                ClientLogoUrl = "https://test.com/logo.png",
                AllowRememberConsent = true,
                IdentityScopes = new List<ScopeViewModel>(),
                ApiScopes = new List<ScopeViewModel>()
            };

            // Assert
            Assert.Equal("Test Client", model.ClientName);
            Assert.Equal("https://test.com", model.ClientUrl);
            Assert.Equal("https://test.com/logo.png", model.ClientLogoUrl);
            Assert.True(model.AllowRememberConsent);
            Assert.NotNull(model.IdentityScopes);
            Assert.NotNull(model.ApiScopes);
        }

        [Fact]
        public void ConsentViewModel_ShouldHandleNullScopes()
        {
            // Arrange
            var model = new ConsentViewModel();

            // Assert
            Assert.Null(model.IdentityScopes);
            Assert.Null(model.ApiScopes);
        }

        [Fact]
        public void ConsentViewModel_ShouldHandleEmptyScopes()
        {
            // Arrange
            var model = new ConsentViewModel
            {
                IdentityScopes = new List<ScopeViewModel>(),
                ApiScopes = new List<ScopeViewModel>()
            };

            // Assert
            Assert.Empty(model.IdentityScopes);
            Assert.Empty(model.ApiScopes);
        }
    }
}
