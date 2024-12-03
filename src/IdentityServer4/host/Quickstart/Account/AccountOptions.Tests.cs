using System;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer4.UnitTests.Quickstart.Account
{
    public class AccountOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Assert
            Assert.True(AccountOptions.AllowLocalLogin);
            Assert.True(AccountOptions.AllowRememberLogin);
            Assert.Equal(TimeSpan.FromDays(30), AccountOptions.RememberMeLoginDuration);
            Assert.True(AccountOptions.ShowLogoutPrompt);
            Assert.False(AccountOptions.AutomaticRedirectAfterSignOut);
            Assert.Equal("Invalid username or password", AccountOptions.InvalidCredentialsErrorMessage);
        }

        [Fact]
        public void RememberMeLoginDuration_ShouldBe30Days()
        {
            // Arrange
            var expectedDuration = TimeSpan.FromDays(30);

            // Assert
            Assert.Equal(expectedDuration.TotalDays, AccountOptions.RememberMeLoginDuration.TotalDays);
            Assert.Equal(expectedDuration.TotalHours, AccountOptions.RememberMeLoginDuration.TotalHours);
        }
    }
}
