using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Consent
{
    public class ConsentOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Assert
            Assert.True(ConsentOptions.EnableOfflineAccess);
            Assert.Equal("Offline Access", ConsentOptions.OfflineAccessDisplayName);
            Assert.Equal("Access to your applications and resources, even when you are offline", ConsentOptions.OfflineAccessDescription);
            Assert.Equal("You must pick at least one permission", ConsentOptions.MustChooseOneErrorMessage);
            Assert.Equal("Invalid selection", ConsentOptions.InvalidSelectionErrorMessage);
        }
    }
}
