using System;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ResourceOwnerPasswordValidationContextTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var context = new ResourceOwnerPasswordValidationContext();

            // Assert
            context.UserName.Should().BeNull();
            context.Password.Should().BeNull();
            context.Request.Should().BeNull();
            context.Result.Should().NotBeNull();
            context.Result.Error.Should().Be(TokenRequestErrors.InvalidGrant);
        }

        [Fact]
        public void SettingProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext();
            var request = new ValidatedTokenRequest();

            // Act
            context.UserName = "testuser";
            context.Password = "testpass";
            context.Request = request;
            context.Result = new GrantValidationResult("subject", "custom");

            // Assert
            context.UserName.Should().Be("testuser");
            context.Password.Should().Be("testpass");
            context.Request.Should().BeSameAs(request);
            context.Result.Subject.Should().Be("subject");
            context.Result.GrantType.Should().Be("custom");
        }
    }
}
