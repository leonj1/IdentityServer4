using System;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.UnitTests.Models.AccountViewModels
{
    public class LoginWith2faViewModelTests
    {
        [Fact]
        public void TwoFactorCode_ValidationAttributes_AreCorrect()
        {
            // Arrange
            var model = new LoginWith2faViewModel();
            var type = typeof(LoginWith2faViewModel);
            var property = type.GetProperty("TwoFactorCode");

            // Act & Assert
            Assert.True(Attribute.IsDefined(property, typeof(RequiredAttribute)));
            Assert.True(Attribute.IsDefined(property, typeof(StringLengthAttribute)));
            Assert.True(Attribute.IsDefined(property, typeof(DataTypeAttribute)));
            Assert.True(Attribute.IsDefined(property, typeof(DisplayAttribute)));
        }

        [Theory]
        [InlineData("123456", true)]  // Valid 6 chars
        [InlineData("1234567", true)] // Valid 7 chars
        [InlineData("12345", false)]  // Too short
        [InlineData("12345678", false)] // Too long
        public void TwoFactorCode_Length_Validation(string code, bool shouldBeValid)
        {
            // Arrange
            var model = new LoginWith2faViewModel { TwoFactorCode = code };
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.Equal(shouldBeValid, isValid);
        }

        [Fact]
        public void RememberMachine_DefaultValue_IsFalse()
        {
            // Arrange & Act
            var model = new LoginWith2faViewModel();

            // Assert
            Assert.False(model.RememberMachine);
        }

        [Fact]
        public void RememberMe_DefaultValue_IsFalse()
        {
            // Arrange & Act
            var model = new LoginWith2faViewModel();

            // Assert
            Assert.False(model.RememberMe);
        }
    }
}
