using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ResourceOwnerPasswordValidatorTests
    {
        private class TestResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
        {
            public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
            {
                // Simulate validation logic
                if (context.UserName == "validuser" && context.Password == "validpass")
                {
                    context.Result = new GrantValidationResult("123", "password");
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
                }

                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task ValidCredentials_ShouldSucceed()
        {
            // Arrange
            var validator = new TestResourceOwnerPasswordValidator();
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "validuser",
                Password = "validpass"
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("123");
            context.Result.GrantType.Should().Be("password");
        }

        [Fact]
        public async Task InvalidCredentials_ShouldFail()
        {
            // Arrange
            var validator = new TestResourceOwnerPasswordValidator();
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "invaliduser",
                Password = "invalidpass"
            };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(TokenRequestErrors.InvalidGrant);
        }
    }
}
