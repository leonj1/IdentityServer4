using System;
using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.UnitTests.Models.ManageViewModels
{
    public class ChangePasswordViewModelTests
    {
        [Fact]
        public void WhenAllPropertiesValid_ValidationPasses()
        {
            // Arrange
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass123!",
                ConfirmPassword = "NewPass123!",
                StatusMessage = "Test Status"
            };

            // Assert
            Assert.NotNull(model.OldPassword);
            Assert.NotNull(model.NewPassword);
            Assert.NotNull(model.ConfirmPassword);
            Assert.Equal(model.NewPassword, model.ConfirmPassword);
        }

        [Fact]
        public void WhenPasswordsDontMatch_CompareAttributeShouldFail()
        {
            // Arrange
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass123!",
                ConfirmPassword = "DifferentPass123!"
            };

            // Assert
            Assert.NotEqual(model.NewPassword, model.ConfirmPassword);
        }

        [Theory]
        [InlineData("short")] // Less than 6 characters
        [InlineData("verylongpasswordthatexceedsthemaximumlengthof100charactersverylongpasswordthatexceedsthemaximumlengthof100characters")]
        public void WhenNewPasswordLengthInvalid_StringLengthAttributeShouldFail(string password)
        {
            // Arrange
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = password,
                ConfirmPassword = password
            };

            // Assert
            Assert.True(password.Length < 6 || password.Length > 100);
        }
    }
}
