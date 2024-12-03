using System;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.UnitTests.Models.AccountViewModels
{
    public class ExternalLoginViewModelTests
    {
        [Fact]
        public void Email_Required_Validation()
        {
            // Arrange
            var model = new ExternalLoginViewModel();

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@.com")]
        [InlineData("test@")]
        [InlineData("@test.com")]
        public void Email_Format_Validation(string invalidEmail)
        {
            // Arrange
            var model = new ExternalLoginViewModel { Email = invalidEmail };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(() => 
                Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user@domain.co.uk")]
        [InlineData("name.surname@company.org")]
        public void Valid_Email_Addresses(string validEmail)
        {
            // Arrange
            var model = new ExternalLoginViewModel { Email = validEmail };

            // Act & Assert
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
