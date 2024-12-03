using System;
using Xunit;
using IdentityServer4.Models.AccountViewModels;

namespace IdentityServer4.UnitTests.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModelTests
    {
        [Fact]
        public void RecoveryCode_Required_Validation()
        {
            // Arrange
            var model = new LoginWithRecoveryCodeViewModel();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            
            // Act & Assert
            Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>(
                () => Validator.ValidateObject(model, context, validateAllProperties: true));
        }

        [Fact]
        public void RecoveryCode_Sets_And_Gets_Correctly()
        {
            // Arrange
            var model = new LoginWithRecoveryCodeViewModel();
            var testCode = "TEST-RECOVERY-CODE";

            // Act
            model.RecoveryCode = testCode;

            // Assert
            Assert.Equal(testCode, model.RecoveryCode);
        }

        [Fact]
        public void RecoveryCode_DataType_Is_Text()
        {
            // Arrange
            var propertyInfo = typeof(LoginWithRecoveryCodeViewModel).GetProperty("RecoveryCode");
            var dataTypeAttribute = (System.ComponentModel.DataAnnotations.DataTypeAttribute)Attribute
                .GetCustomAttribute(propertyInfo, typeof(System.ComponentModel.DataAnnotations.DataTypeAttribute));

            // Assert
            Assert.NotNull(dataTypeAttribute);
            Assert.Equal(System.ComponentModel.DataAnnotations.DataType.Text, dataTypeAttribute.DataType);
        }

        [Fact]
        public void RecoveryCode_Display_Name_Is_Set()
        {
            // Arrange
            var propertyInfo = typeof(LoginWithRecoveryCodeViewModel).GetProperty("RecoveryCode");
            var displayAttribute = (System.ComponentModel.DataAnnotations.DisplayAttribute)Attribute
                .GetCustomAttribute(propertyInfo, typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));

            // Assert
            Assert.NotNull(displayAttribute);
            Assert.Equal("Recovery Code", displayAttribute.Name);
        }
    }
}
