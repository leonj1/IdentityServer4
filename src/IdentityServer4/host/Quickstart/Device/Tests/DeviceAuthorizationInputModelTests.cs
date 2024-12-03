using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class DeviceAuthorizationInputModelTests
    {
        [Fact]
        public void UserCode_ShouldSetAndGetValue()
        {
            // Arrange
            var model = new DeviceAuthorizationInputModel();
            var expectedCode = "TEST123";

            // Act
            model.UserCode = expectedCode;

            // Assert
            Assert.Equal(expectedCode, model.UserCode);
        }

        [Fact]
        public void DeviceAuthorizationInputModel_InheritsFromConsentInputModel()
        {
            // Arrange & Act
            var model = new DeviceAuthorizationInputModel();

            // Assert
            Assert.IsAssignableFrom<ConsentInputModel>(model);
        }
    }
}
