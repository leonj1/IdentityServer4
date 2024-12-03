using IdentityServerHost.Quickstart.UI;
using Xunit;

namespace IdentityServer.UnitTests.Quickstart.Account
{
    public class LogoutViewModelTests
    {
        [Fact]
        public void ShowLogoutPrompt_DefaultValue_ShouldBeTrue()
        {
            // Arrange
            var viewModel = new LogoutViewModel();

            // Act & Assert
            Assert.True(viewModel.ShowLogoutPrompt);
        }

        [Fact]
        public void ShowLogoutPrompt_WhenSet_ShouldReturnSetValue()
        {
            // Arrange
            var viewModel = new LogoutViewModel
            {
                ShowLogoutPrompt = false
            };

            // Act & Assert
            Assert.False(viewModel.ShowLogoutPrompt);
        }

        [Fact]
        public void LogoutViewModel_ShouldInheritFromLogoutInputModel()
        {
            // Arrange
            var viewModel = new LogoutViewModel();

            // Act & Assert
            Assert.IsAssignableFrom<LogoutInputModel>(viewModel);
        }
    }
}
