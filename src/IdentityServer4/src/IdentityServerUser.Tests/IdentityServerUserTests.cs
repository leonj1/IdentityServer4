using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Xunit;

namespace IdentityServer4.Tests
{
    public class IdentityServerUserTests
    {
        [Fact]
        public void Constructor_WithValidSubjectId_CreatesInstance()
        {
            // Arrange & Act
            var user = new IdentityServerUser("123");

            // Assert
            Assert.Equal("123", user.SubjectId);
        }

        [Fact]
        public void Constructor_WithNullSubjectId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new IdentityServerUser(null));
        }

        [Fact]
        public void CreatePrincipal_WithMinimalData_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123");

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            Assert.Single(principal.Claims.Where(c => c.Type == JwtClaimTypes.Subject && c.Value == "123"));
        }

        [Fact]
        public void CreatePrincipal_WithAllProperties_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                DisplayName = "Test User",
                IdentityProvider = "local",
                AuthenticationTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                AuthenticationMethods = new[] { "pwd" },
                AdditionalClaims = new[] { new Claim("custom", "value") }
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            var claims = principal.Claims.ToList();
            Assert.Contains(claims, c => c.Type == JwtClaimTypes.Subject && c.Value == "123");
            Assert.Contains(claims, c => c.Type == JwtClaimTypes.Name && c.Value == "Test User");
            Assert.Contains(claims, c => c.Type == JwtClaimTypes.IdentityProvider && c.Value == "local");
            Assert.Contains(claims, c => c.Type == JwtClaimTypes.AuthenticationTime);
            Assert.Contains(claims, c => c.Type == JwtClaimTypes.AuthenticationMethod && c.Value == "pwd");
            Assert.Contains(claims, c => c.Type == "custom" && c.Value == "value");
        }

        [Fact]
        public void CreatePrincipal_WithDuplicateClaims_RemovesDuplicates()
        {
            // Arrange
            var user = new IdentityServerUser("123");
            user.AdditionalClaims = new[]
            {
                new Claim("custom", "value"),
                new Claim("custom", "value")
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.Single(principal.Claims.Where(c => c.Type == "custom" && c.Value == "value"));
        }
    }

    public class IdentityServerUserConstructorTests
    {
        [Fact]
        public void Constructor_WithValidSubjectId_CreatesInstance()
        {
            // Arrange & Act
            var user = new IdentityServerUser("123");

            // Assert
            Assert.Equal("123", user.SubjectId);
        }
    }

    public class IdentityServerUserConstructorTestsRefactored
    {
        [Fact]
        public void Constructor_WithValidSubjectId_CreatesInstance()
        {
            // Arrange & Act
            var user = new IdentityServerUser("123");

            // Assert
            Assert.Equal("123", user.SubjectId);
        }
    }
}
