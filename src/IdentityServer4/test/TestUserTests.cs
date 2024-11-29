using System;
using System.Security.Claims;
using System.Collections.Generic;
using Xunit;
using IdentityServer4.Test;
using IdentityModel;

namespace IdentityServer4.UnitTests
{
    public class TestUserTests
    {
        [Fact]
        public void TestUser_Constructor_ShouldInitializeDefaultValues()
        {
            // Arrange & Act
            var user = new TestUser();

            // Assert
            Assert.True(user.IsActive);
            Assert.NotNull(user.Claims);
            Assert.IsType<HashSet<Claim>>(user.Claims);
        }

        [Fact]
        public void TestUser_Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new TestUser
            {
                SubjectId = "123",
                Username = "testuser",
                Password = "password",
                ProviderName = "test-provider",
                ProviderSubjectId = "provider-123",
                IsActive = false
            };

            // Act & Assert
            Assert.Equal("123", user.SubjectId);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("password", user.Password);
            Assert.Equal("test-provider", user.ProviderName);
            Assert.Equal("provider-123", user.ProviderSubjectId);
            Assert.False(user.IsActive);
        }

        [Fact]
        public void TestUser_Claims_ShouldHandleClaimsCorrectly()
        {
            // Arrange
            var user = new TestUser();
            var claim1 = new Claim(JwtClaimTypes.Name, "John Doe");
            var claim2 = new Claim(JwtClaimTypes.Email, "john@example.com");

            // Act
            user.Claims.Add(claim1);
            user.Claims.Add(claim2);

            // Assert
            Assert.Contains(claim1, user.Claims);
            Assert.Contains(claim2, user.Claims);
            Assert.Equal(2, user.Claims.Count);
        }

        [Fact]
        public void TestUser_Claims_ShouldPreventDuplicateClaims()
        {
            // Arrange
            var user = new TestUser();
            var claim1 = new Claim(JwtClaimTypes.Name, "John Doe");
            var claim2 = new Claim(JwtClaimTypes.Name, "John Doe");

            // Act
            user.Claims.Add(claim1);
            user.Claims.Add(claim2);

            // Assert
            Assert.Single(user.Claims);
        }
    }
}
