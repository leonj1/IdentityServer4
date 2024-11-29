using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer4.UnitTests.Quickstart.Account
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
                    new ExternalProvider { AuthenticationScheme = "scheme1", DisplayName = "Provider1" },
                    new ExternalProvider { AuthenticationScheme = "scheme2", DisplayName = null },
                    new ExternalProvider { AuthenticationScheme = "scheme3", DisplayName = "" }
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
                    new ExternalProvider { AuthenticationScheme = "scheme1", DisplayName = "Provider1" }
                }
            };

            // Act & Assert
            Assert.True(model.IsExternalLoginOnly);
        }

        [Fact]
        public void IsExternalLoginOnly_WithMultipleProviders_ReturnsFalse()
        {
            // Arrange
            var model = new LoginViewModel
            {
                EnableLocalLogin = false,
                ExternalProviders = new[]
                {
                    new ExternalProvider { AuthenticationScheme = "scheme1", DisplayName = "Provider1" },
                    new ExternalProvider { AuthenticationScheme = "scheme2", DisplayName = "Provider2" }
                }
            };

            // Act & Assert
            Assert.False(model.IsExternalLoginOnly);
        }

        [Fact]
        public void ExternalLoginScheme_WithSingleProvider_ReturnsScheme()
        {
            // Arrange
            var model = new LoginViewModel
            {
                EnableLocalLogin = false,
                ExternalProviders = new[]
                {
                    new ExternalProvider { AuthenticationScheme = "scheme1", DisplayName = "Provider1" }
                }
            };

            // Act & Assert
            Assert.Equal("scheme1", model.ExternalLoginScheme);
        }

        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var model = new LoginViewModel();

            // Assert
            Assert.True(model.AllowRememberLogin);
            Assert.True(model.EnableLocalLogin);
            Assert.Empty(model.ExternalProviders);
        }
    }
}
