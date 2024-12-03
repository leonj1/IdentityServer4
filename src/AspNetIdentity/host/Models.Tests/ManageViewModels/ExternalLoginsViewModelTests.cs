using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.Models.Tests.ManageViewModels
{
    public class ExternalLoginsViewModelTests
    {
        [Fact]
        public void ExternalLoginsViewModel_Properties_InitializeCorrectly()
        {
            // Arrange
            var model = new ExternalLoginsViewModel
            {
                CurrentLogins = new List<UserLoginInfo>(),
                OtherLogins = new List<AuthenticationScheme>(),
                ShowRemoveButton = true,
                StatusMessage = "Test Status"
            };

            // Assert
            Assert.NotNull(model.CurrentLogins);
            Assert.NotNull(model.OtherLogins);
            Assert.True(model.ShowRemoveButton);
            Assert.Equal("Test Status", model.StatusMessage);
        }

        [Fact]
        public void ExternalLoginsViewModel_DefaultValues_AreCorrect()
        {
            // Arrange
            var model = new ExternalLoginsViewModel();

            // Assert
            Assert.Null(model.CurrentLogins);
            Assert.Null(model.OtherLogins);
            Assert.False(model.ShowRemoveButton);
            Assert.Null(model.StatusMessage);
        }
    }
}
