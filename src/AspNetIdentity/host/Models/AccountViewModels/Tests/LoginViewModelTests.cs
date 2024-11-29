using System;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.Tests.Models.AccountViewModels
{
    public class LoginViewModelTests
    {
        [Fact]
        public void LoginViewModel_ValidData_ShouldSetProperties()
        {
            // Arrange
            var email = "test@example.com";
            var password = "TestPassword123!";
            var rememberMe = true;

            // Act
            var model = new LoginViewModel
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            };

            // Assert
            Assert.Equal(email, model.Email);
            Assert.Equal(password, model.Password);
            Assert.True(model.RememberMe);
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        [InlineData("invalid@")]
        public void LoginViewModel_InvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = invalidEmail,
                Password = "TestPassword123!"
            };

            // Act & Assert
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, validationContext, validateAllProperties: true));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void LoginViewModel_InvalidPassword_ShouldFailValidation(string invalidPassword)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = invalidPassword
            };

            // Act & Assert
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, validationContext, validateAllProperties: true));
        }
    }
}
