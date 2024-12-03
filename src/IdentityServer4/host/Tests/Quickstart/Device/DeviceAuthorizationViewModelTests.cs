using IdentityServerHost.Quickstart.UI;
using Xunit;

namespace IdentityServer4.UnitTests.Quickstart.Device
{
    public class DeviceAuthorizationViewModelTests
    {
        [Fact]
        public void DeviceAuthorizationViewModel_Properties_ShouldBeSettable()
        {
            // Arrange
            var viewModel = new DeviceAuthorizationViewModel
            {
                UserCode = "123456",
                ConfirmUserCode = true
            };

            // Assert
            Assert.Equal("123456", viewModel.UserCode);
            Assert.True(viewModel.ConfirmUserCode);
        }

        [Fact]
        public void DeviceAuthorizationViewModel_InheritsFrom_ConsentViewModel()
        {
            // Arrange
            var viewModel = new DeviceAuthorizationViewModel();

            // Assert
            Assert.IsAssignableFrom<ConsentViewModel>(viewModel);
        }
    }
}
