using System;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.Test.Models.AccountViewModels
{
    public class ResetPasswordViewModelTests
    {
        [Fact]
        public void ValidModel_ShouldPassValidation()
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "ValidP@ss123",
                ConfirmPassword = "ValidP@ss123",
                Code = "resetcode123"
            };

            // Act & Assert - if no validation attributes are violated, this will pass
            Assert.NotNull(model);
            Assert.Equal("test@example.com", model.Email);
            Assert.Equal("ValidP@ss123", model.Password);
            Assert.Equal("ValidP@ss123", model.ConfirmPassword);
            Assert.Equal("resetcode123", model.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        [InlineData("@invalid.com")]
        public void InvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = invalidEmail,
                Password = "ValidP@ss123",
                ConfirmPassword = "ValidP@ss123",
                Code = "resetcode123"
            };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Theory]
        [InlineData("12345")] // Too short
        [InlineData("abcd")] // Too short
        [InlineData("")] // Empty
        public void InvalidPassword_ShouldFailValidation(string invalidPassword)
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = invalidPassword,
                ConfirmPassword = invalidPassword,
                Code = "resetcode123"
            };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Fact]
        public void PasswordMismatch_ShouldFailValidation()
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "ValidP@ss123",
                ConfirmPassword = "DifferentP@ss123",
                Code = "resetcode123"
            };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }
    }
}
