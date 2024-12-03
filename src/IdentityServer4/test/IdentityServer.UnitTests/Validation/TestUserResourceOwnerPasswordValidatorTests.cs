using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TestUserResourceOwnerPasswordValidatorTests
    {
        private readonly TestUserStore _userStore;
        private readonly TestUserResourceOwnerPasswordValidator _validator;
        private readonly ISystemClock _clock;
        
        public TestUserResourceOwnerPasswordValidatorTests()
        {
            _userStore = new TestUserStore();
            _clock = new SystemClock();
            _validator = new TestUserResourceOwnerPasswordValidator(_userStore, _clock);
            
            // Add a test user
            _userStore.Users.Add(new TestUser
            {
                SubjectId = "123",
                Username = "testuser",
                Password = "password",
                Claims = new[] 
                { 
                    new Claim("name", "Test User"),
                    new Claim("email", "test@test.com")
                }
            });
        }

        [Fact]
        public async Task ValidCredentials_ShouldReturnSuccessResult()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "password"
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.Should().NotBeNull();
            context.Result.IsError.Should().BeFalse();
            context.Result.Subject.Should().Be("123");
            context.Result.AuthenticationMethod.Should().Be(OidcConstants.AuthenticationMethods.Password);
        }

        [Fact]
        public async Task InvalidCredentials_ShouldReturnNullResult()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "testuser",
                Password = "wrongpassword"
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public async Task NonexistentUser_ShouldReturnNullResult()
        {
            // Arrange
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "nonexistentuser",
                Password = "password"
            };

            // Act
            await _validator.ValidateAsync(context);

            // Assert
            context.Result.Should().BeNull();
        }
    }
}
