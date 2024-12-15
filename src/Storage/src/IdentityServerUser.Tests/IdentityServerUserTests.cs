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
        public void CreatePrincipal_WithEmptyAuthenticationMethods_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                AuthenticationMethods = Array.Empty<string>()
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            Assert.Empty(principal.Claims.Where(c => c.Type == JwtClaimTypes.AuthenticationMethod));
        }

        [Fact]
        public void CreatePrincipal_WithNullAuthenticationMethods_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                AuthenticationMethods = null
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            Assert.Empty(principal.Claims.Where(c => c.Type == JwtClaimTypes.AuthenticationMethod));
        }

        [Fact]
        public void CreatePrincipal_WithNullAdditionalClaims_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                AdditionalClaims = null
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            Assert.Single(principal.Claims.Where(c => c.Type == JwtClaimTypes.Subject));
        }

        [Fact]
        public void Constructor_WithEmptySubjectId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new IdentityServerUser(string.Empty));
        }

        [Fact]
        public void CreatePrincipal_WithMultipleAuthenticationMethods_CreatesValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                AuthenticationMethods = new[] { "pwd", "2fa", "external" }
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            Assert.NotNull(principal);
            var authMethods = principal.Claims
                .Where(c => c.Type == JwtClaimTypes.AuthenticationMethod)
                .Select(c => c.Value)
                .ToList();
            Assert.Equal(3, authMethods.Count);
            Assert.Contains("pwd", authMethods);
            Assert.Contains("2fa", authMethods);
            Assert.Contains("external", authMethods);
        }
    }

    public class ConstructorWithValidSubjectIdTests
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
