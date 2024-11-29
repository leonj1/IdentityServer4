using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Xunit;

namespace IdentityServer4.UnitTests
{
    public class IdentityServerUserTests
    {
        [Fact]
        public void Constructor_WithValidSubjectId_ShouldCreateInstance()
        {
            var user = new IdentityServerUser("123");
            Assert.Equal("123", user.SubjectId);
        }

        [Fact]
        public void Constructor_WithNullOrEmptySubjectId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new IdentityServerUser(null));
            Assert.Throws<ArgumentException>(() => new IdentityServerUser(string.Empty));
        }

        [Fact]
        public void CreatePrincipal_WithMinimalData_ShouldCreateValidPrincipal()
        {
            var user = new IdentityServerUser("123");
            var principal = user.CreatePrincipal();

            Assert.NotNull(principal);
            Assert.Single(principal.Identities);
            Assert.Equal("123", principal.FindFirst(JwtClaimTypes.Subject)?.Value);
        }

        [Fact]
        public void CreatePrincipal_WithAllProperties_ShouldCreatePrincipalWithAllClaims()
        {
            var user = new IdentityServerUser("123")
            {
                DisplayName = "Test User",
                IdentityProvider = "local",
                AuthenticationTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                AuthenticationMethods = new[] { "pwd", "2fa" },
                AdditionalClaims = new[] { new Claim("custom", "value") }
            };

            var principal = user.CreatePrincipal();

            Assert.Equal("Test User", principal.FindFirst(JwtClaimTypes.Name)?.Value);
            Assert.Equal("local", principal.FindFirst(JwtClaimTypes.IdentityProvider)?.Value);
            Assert.Equal("1672574400", principal.FindFirst(JwtClaimTypes.AuthenticationTime)?.Value);
            Assert.Equal(2, principal.Claims.Where(c => c.Type == JwtClaimTypes.AuthenticationMethod).Count());
            Assert.Contains(principal.Claims, c => c.Type == "custom" && c.Value == "value");
        }

        [Fact]
        public void CreatePrincipal_WithDuplicateClaims_ShouldRemoveDuplicates()
        {
            var user = new IdentityServerUser("123");
            user.AdditionalClaims = new[]
            {
                new Claim("custom", "value"),
                new Claim("custom", "value")
            };

            var principal = user.CreatePrincipal();
            Assert.Single(principal.Claims.Where(c => c.Type == "custom"));
        }
    }
}
