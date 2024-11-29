using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Quickstart.Account
{
    public class ExternalProviderTests
    {
        [Fact]
        public void ExternalProvider_Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var provider = new ExternalProvider
            {
                DisplayName = "Test Provider",
                AuthenticationScheme = "TestScheme"
            };

            // Assert
            Assert.Equal("Test Provider", provider.DisplayName);
            Assert.Equal("TestScheme", provider.AuthenticationScheme);
        }

        [Fact]
        public void ExternalProvider_DefaultValues_ShouldBeNull()
        {
            // Arrange
            var provider = new ExternalProvider();

            // Assert
            Assert.Null(provider.DisplayName);
            Assert.Null(provider.AuthenticationScheme);
        }
    }
}
