using System;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Device
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
            Assert.True(viewModel is ConsentViewModel);
        }
    }
}
