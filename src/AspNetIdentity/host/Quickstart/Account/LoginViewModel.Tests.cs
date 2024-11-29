using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class LoginViewModelTests
    {
        [Fact]
        public void VisibleExternalProviders_WithNullDisplayName_ShouldFilterOut()
        {
            // Arrange
            var model = new LoginViewModel
            {
                ExternalProviders = new[]
                {
                    new ExternalProvider { DisplayName = "Provider1", AuthenticationScheme = "scheme1" },
                    new ExternalProvider { DisplayName = null, AuthenticationScheme = "scheme2" },
                    new ExternalProvider { DisplayName = "", AuthenticationScheme = "scheme3" }
                }
            };

            // Act
            var result = model.VisibleExternalProviders.ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Provider1", result[0].DisplayName);
        }

        [Fact]
        public void IsExternalLoginOnly_WithSingleProvider_AndLocalLoginDisabled_ReturnsTrue()
        {
            // Arrange
            var model = new LoginViewModel
            {
                EnableLocalLogin = false,
                ExternalProviders = new[]
                {
                    new ExternalProvider { DisplayName = "Provider1", AuthenticationScheme = "scheme1" }
                }
            };

            // Act & Assert
            Assert.True(model.IsExternalLoginOnly);
        }

        [Fact]
        public void ExternalLoginScheme_WithSingleProvider_ReturnsCorrectScheme()
        {
            // Arrange
            var model = new LoginViewModel
            {
                EnableLocalLogin = false,
                ExternalProviders = new[]
                {
                    new ExternalProvider { DisplayName = "Provider1", AuthenticationScheme = "scheme1" }
                }
            };

            // Act & Assert
            Assert.Equal("scheme1", model.ExternalLoginScheme);
        }

        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var model = new LoginViewModel();

            // Assert
            Assert.True(model.AllowRememberLogin);
            Assert.True(model.EnableLocalLogin);
            Assert.Empty(model.ExternalProviders);
        }
    }
}
