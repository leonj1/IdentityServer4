using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.UnitTests.Models.AccountViewModels
{
    public class RegisterViewModelTests
    {
        [Fact]
        public void ValidModel_PassesValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("", "Email is required")]
        [InlineData("notanemail", "Invalid email format")]
        [InlineData("test@", "Invalid email format")]
        public void InvalidEmail_FailsValidation(string email, string expectedError)
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, 
                v => v.ErrorMessage != null && 
                     v.ErrorMessage.Contains(expectedError, StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData("", "Password is required")]
        [InlineData("12345", "The Password must be at least 6 characters long")]
        public void InvalidPassword_FailsValidation(string password, string expectedError)
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = password,
                ConfirmPassword = password
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults, 
                v => v.ErrorMessage != null && 
                     v.ErrorMessage.Contains(expectedError, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void NonMatchingPasswords_FailsValidation()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword123!"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.Contains(validationResults,
                v => v.ErrorMessage != null &&
                     v.ErrorMessage.Contains("password and confirmation password do not match", 
                         StringComparison.OrdinalIgnoreCase));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
