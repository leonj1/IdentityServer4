using System;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer4.UnitTests.Models
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_ShouldInheritFromIdentityUser()
        {
            // Arrange
            var user = new ApplicationUser();

            // Assert
            Assert.True(user is Microsoft.AspNetCore.Identity.IdentityUser);
        }

        [Fact]
        public void ApplicationUser_Properties_ShouldBeInitialized()
        {
            // Arrange & Act
            var user = new ApplicationUser
            {
                UserName = "testuser",
                Email = "test@example.com"
            };

            // Assert
            Assert.Equal("testuser", user.UserName);
            Assert.Equal("test@example.com", user.Email);
        }
    }
}
