using System;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Consent
{
    public class ScopeViewModelTests
    {
        [Fact]
        public void ScopeViewModel_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var scope = new ScopeViewModel
            {
                Value = "api1",
                DisplayName = "API 1",
                Description = "Test API",
                Emphasize = true,
                Required = true,
                Checked = true
            };

            // Assert
            Assert.Equal("api1", scope.Value);
            Assert.Equal("API 1", scope.DisplayName);
            Assert.Equal("Test API", scope.Description);
            Assert.True(scope.Emphasize);
            Assert.True(scope.Required);
            Assert.True(scope.Checked);
        }

        [Fact]
        public void ScopeViewModel_DefaultValues_AreCorrect()
        {
            // Arrange
            var scope = new ScopeViewModel();

            // Assert
            Assert.Null(scope.Value);
            Assert.Null(scope.DisplayName);
            Assert.Null(scope.Description);
            Assert.False(scope.Emphasize);
            Assert.False(scope.Required);
            Assert.False(scope.Checked);
        }
    }
}
