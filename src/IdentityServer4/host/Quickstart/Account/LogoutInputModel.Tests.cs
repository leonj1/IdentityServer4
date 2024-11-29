using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class LogoutInputModelTests
    {
        [Fact]
        public void LogoutId_CanBeSetAndRetrieved()
        {
            // Arrange
            var model = new LogoutInputModel();
            var expectedLogoutId = "test-logout-id";

            // Act
            model.LogoutId = expectedLogoutId;

            // Assert
            Assert.Equal(expectedLogoutId, model.LogoutId);
        }

        [Fact]
        public void LogoutId_DefaultsToNull()
        {
            // Arrange & Act
            var model = new LogoutInputModel();

            // Assert
            Assert.Null(model.LogoutId);
        }
    }
}
