using System;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using Xunit;

namespace IdentityServer4.UnitTests
{
    public class IdentityServerUserTests
    {
        [Fact]
        public void Constructor_WithValidSubjectId_ShouldCreateInstance()
        {
            // Arrange & Act
            var user = new IdentityServerUser("123");

            // Assert
            user.SubjectId.Should().Be("123");
        }

        [Fact]
        public void Constructor_WithNullSubjectId_ShouldThrowException()
        {
            // Arrange & Act
            Action act = () => new IdentityServerUser(null);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("SubjectId is mandatory*");
        }

        [Fact]
        public void CreatePrincipal_WithBasicInfo_ShouldCreateValidPrincipal()
        {
            // Arrange
            var user = new IdentityServerUser("123")
            {
                DisplayName = "Test User",
                IdentityProvider = "local",
                AuthenticationTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            principal.Should().NotBeNull();
            principal.Identity.Should().NotBeNull();
            principal.Identity.IsAuthenticated.Should().BeTrue();
            principal.Identity.AuthenticationType.Should().Be(Constants.IdentityServerAuthenticationType);
            
            var claims = principal.Claims.ToList();
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Subject && c.Value == "123");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Name && c.Value == "Test User");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.IdentityProvider && c.Value == "local");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.AuthenticationTime && c.Value == "1672574400");
        }

        [Fact]
        public void CreatePrincipal_WithAuthenticationMethods_ShouldIncludeAmrClaims()
        {
            // Arrange
            var user = new IdentityServerUser("123");
            user.AuthenticationMethods.Add("pwd");
            user.AuthenticationMethods.Add("otp");

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            var amrClaims = principal.Claims.Where(c => c.Type == JwtClaimTypes.AuthenticationMethod).ToList();
            amrClaims.Should().HaveCount(2);
            amrClaims.Select(c => c.Value).Should().Contain(new[] { "pwd", "otp" });
        }

        [Fact]
        public void CreatePrincipal_WithAdditionalClaims_ShouldIncludeAllClaims()
        {
            // Arrange
            var user = new IdentityServerUser("123");
            user.AdditionalClaims.Add(new Claim("role", "admin"));
            user.AdditionalClaims.Add(new Claim("permission", "read"));

            // Act
            var principal = user.CreatePrincipal();

            // Assert
            var claims = principal.Claims.ToList();
            claims.Should().Contain(c => c.Type == "role" && c.Value == "admin");
            claims.Should().Contain(c => c.Type == "permission" && c.Value == "read");
        }
    }
}
