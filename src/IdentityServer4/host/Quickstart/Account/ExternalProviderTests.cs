using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer4.UnitTests.Quickstart.Account
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
        public void ExternalProvider_DefaultConstructor_ShouldCreateEmptyInstance()
        {
            // Arrange & Act
            var provider = new ExternalProvider();

            // Assert
            Assert.Null(provider.DisplayName);
            Assert.Null(provider.AuthenticationScheme);
        }
    }
}
