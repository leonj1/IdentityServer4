using System;
using Xunit;
using IdentityServer4.Models.ManageViewModels;

namespace IdentityServer4.Tests.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModelTests
    {
        [Fact]
        public void Code_ValidationAttributes_AreCorrect()
        {
            // Arrange
            var model = new EnableAuthenticatorViewModel();

            // Act & Assert
            var codeProperty = typeof(EnableAuthenticatorViewModel).GetProperty("Code");
            var requiredAttr = (RequiredAttribute)Attribute.GetCustomAttribute(codeProperty, typeof(RequiredAttribute));
            var stringLengthAttr = (StringLengthAttribute)Attribute.GetCustomAttribute(codeProperty, typeof(StringLengthAttribute));
            var dataTypeAttr = (DataTypeAttribute)Attribute.GetCustomAttribute(codeProperty, typeof(DataTypeAttribute));
            var displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(codeProperty, typeof(DisplayAttribute));

            Assert.NotNull(requiredAttr);
            Assert.NotNull(stringLengthAttr);
            Assert.Equal(7, stringLengthAttr.MaximumLength);
            Assert.Equal(6, stringLengthAttr.MinimumLength);
            Assert.Equal(DataType.Text, dataTypeAttr.DataType);
            Assert.Equal("Verification Code", displayAttr.Name);
        }

        [Fact]
        public void SharedKey_IsReadOnly()
        {
            // Arrange & Act
            var sharedKeyProperty = typeof(EnableAuthenticatorViewModel).GetProperty("SharedKey");
            var readOnlyAttr = (ReadOnlyAttribute)Attribute.GetCustomAttribute(sharedKeyProperty, typeof(ReadOnlyAttribute));

            // Assert
            Assert.NotNull(readOnlyAttr);
            Assert.True(readOnlyAttr.IsReadOnly);
        }

        [Fact]
        public void Properties_CanBeSetAndGet()
        {
            // Arrange
            var model = new EnableAuthenticatorViewModel
            {
                Code = "123456",
                SharedKey = "TestKey",
                AuthenticatorUri = "otpauth://test"
            };

            // Act & Assert
            Assert.Equal("123456", model.Code);
            Assert.Equal("TestKey", model.SharedKey);
            Assert.Equal("otpauth://test", model.AuthenticatorUri);
        }
    }
}
