using System;
using Xunit;
using IdentityServerHost.Quickstart.UI;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class LoginInputModelTests
    {
        [Fact]
        public void LoginInputModel_ValidData_PassesValidation()
        {
            // Arrange
            var model = new LoginInputModel
            {
                Username = "testuser",
                Password = "testpass",
                RememberLogin = true,
                ReturnUrl = "/home"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, 
                new ValidationContext(model), 
                validationResults, 
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LoginInputModel_EmptyUsername_FailsValidation(string username)
        {
            // Arrange
            var model = new LoginInputModel
            {
                Username = username,
                Password = "testpass"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, 
                new ValidationContext(model), 
                validationResults, 
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Username"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LoginInputModel_EmptyPassword_FailsValidation(string password)
        {
            // Arrange
            var model = new LoginInputModel
            {
                Username = "testuser",
                Password = password
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, 
                new ValidationContext(model), 
                validationResults, 
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginInputModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var model = new LoginInputModel();

            // Assert
            Assert.False(model.RememberLogin);
            Assert.Null(model.ReturnUrl);
            Assert.Null(model.Username);
            Assert.Null(model.Password);
        }
    }
}
