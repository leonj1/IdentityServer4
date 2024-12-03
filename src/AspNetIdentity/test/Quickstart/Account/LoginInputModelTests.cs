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
        public void Username_IsRequired()
        {
            // Arrange
            var model = new LoginInputModel { Password = "password" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void Password_IsRequired()
        {
            // Arrange
            var model = new LoginInputModel { Username = "user" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void ValidModel_PassesValidation()
        {
            // Arrange
            var model = new LoginInputModel 
            { 
                Username = "testuser",
                Password = "password123",
                RememberLogin = true,
                ReturnUrl = "/home"
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
