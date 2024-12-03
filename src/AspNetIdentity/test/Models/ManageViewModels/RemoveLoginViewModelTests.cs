using IdentityServer4.Models.ManageViewModels;
using Xunit;

namespace IdentityServer4.UnitTests.Models.ManageViewModels
{
    public class RemoveLoginViewModelTests
    {
        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var model = new RemoveLoginViewModel
            {
                LoginProvider = "TestProvider",
                ProviderKey = "TestKey"
            };

            // Assert
            Assert.Equal("TestProvider", model.LoginProvider);
            Assert.Equal("TestKey", model.ProviderKey);
        }

        [Fact]
        public void DefaultConstructor_ShouldCreateEmptyProperties()
        {
            // Arrange & Act
            var model = new RemoveLoginViewModel();

            // Assert
            Assert.Null(model.LoginProvider);
            Assert.Null(model.ProviderKey);
        }
    }
}
