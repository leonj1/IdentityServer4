using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.UnitTests.Models.ManageViewModels
{
    public class TwoFactorAuthenticationViewModelTests
    {
        [Fact]
        public void DefaultProperties_ShouldHaveExpectedValues()
        {
            // Arrange
            var model = new TwoFactorAuthenticationViewModel();

            // Assert
            Assert.False(model.HasAuthenticator);
            Assert.Equal(0, model.RecoveryCodesLeft);
            Assert.False(model.Is2faEnabled);
        }

        [Fact]
        public void SetProperties_ShouldReturnExpectedValues()
        {
            // Arrange
            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = true,
                RecoveryCodesLeft = 5,
                Is2faEnabled = true
            };

            // Assert
            Assert.True(model.HasAuthenticator);
            Assert.Equal(5, model.RecoveryCodesLeft);
            Assert.True(model.Is2faEnabled);
        }
    }
}
