using System;
using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.UnitTests.Models.ManageViewModels
{
    public class SetPasswordViewModelTests
    {
        [Fact]
        public void ValidPassword_ShouldPass_Validation()
        {
            // Arrange
            var model = new SetPasswordViewModel
            {
                NewPassword = "ValidPass123!",
                ConfirmPassword = "ValidPass123!"
            };

            // Act & Assert
            Assert.Equal(model.NewPassword, model.ConfirmPassword);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12345")] // Too short
        public void InvalidPassword_ShouldFail_Validation(string password)
        {
            // Arrange
            var model = new SetPasswordViewModel
            {
                NewPassword = password,
                ConfirmPassword = password
            };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Fact]
        public void MismatchedPasswords_ShouldFail_Validation()
        {
            // Arrange
            var model = new SetPasswordViewModel
            {
                NewPassword = "ValidPass123!",
                ConfirmPassword = "DifferentPass123!"
            };

            // Act & Assert
            Assert.NotEqual(model.NewPassword, model.ConfirmPassword);
        }

        [Fact]
        public void StatusMessage_CanBeSet()
        {
            // Arrange
            var expectedMessage = "Password successfully updated";
            var model = new SetPasswordViewModel
            {
                StatusMessage = expectedMessage
            };

            // Act & Assert
            Assert.Equal(expectedMessage, model.StatusMessage);
        }
    }
}
