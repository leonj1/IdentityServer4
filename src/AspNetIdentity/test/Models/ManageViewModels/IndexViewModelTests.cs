using System;
using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.Test.Models.ManageViewModels
{
    public class IndexViewModelTests
    {
        [Fact]
        public void IndexViewModel_Properties_SetAndGet_Correctly()
        {
            // Arrange
            var model = new IndexViewModel
            {
                Username = "testuser",
                IsEmailConfirmed = true,
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                StatusMessage = "Test status"
            };

            // Assert
            Assert.Equal("testuser", model.Username);
            Assert.True(model.IsEmailConfirmed);
            Assert.Equal("test@example.com", model.Email);
            Assert.Equal("1234567890", model.PhoneNumber);
            Assert.Equal("Test status", model.StatusMessage);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("")]
        [InlineData(null)]
        public void IndexViewModel_InvalidEmail_FailsValidation(string invalidEmail)
        {
            // Arrange
            var model = new IndexViewModel { Email = invalidEmail };
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            
            // Act & Assert
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(
                () => Validator.ValidateProperty(model.Email, 
                    new System.ComponentModel.DataAnnotations.ValidationContext(model) 
                    { MemberName = nameof(IndexViewModel.Email) }));
        }

        [Theory]
        [InlineData("invalid-phone")]
        public void IndexViewModel_InvalidPhone_FailsValidation(string invalidPhone)
        {
            // Arrange
            var model = new IndexViewModel { PhoneNumber = invalidPhone };
            
            // Act & Assert
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(
                () => Validator.ValidateProperty(model.PhoneNumber, 
                    new System.ComponentModel.DataAnnotations.ValidationContext(model) 
                    { MemberName = nameof(IndexViewModel.PhoneNumber) }));
        }
    }
}
