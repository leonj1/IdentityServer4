using System;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class RefreshTokenTests
    {
        [Fact]
        public void Constructor_DefaultValues_ShouldBeSet()
        {
            var refreshToken = new RefreshToken();
            
            refreshToken.Version.Should().Be(4);
            refreshToken.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            refreshToken.Lifetime.Should().Be(0);
            refreshToken.ConsumedTime.Should().BeNull();
            refreshToken.AccessToken.Should().BeNull();
        }

        [Fact]
        public void Subject_WithAccessTokenClaims_ShouldCreatePrincipalWithClaims()
        {
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token
                {
                    SubjectId = "123",
                    ClientId = "client",
                    Claims = new[] 
                    { 
                        new Claim("name", "Bob"),
                        new Claim("role", "admin")
                    }
                }
            };

            var principal = refreshToken.Subject;

            principal.Should().NotBeNull();
            principal.Claims.Count().Should().Be(3); // Including sub claim
            principal.FindFirst("sub").Value.Should().Be("123");
            principal.FindFirst("name").Value.Should().Be("Bob");
            principal.FindFirst("role").Value.Should().Be("admin");
        }

        [Fact]
        public void Properties_WithValidAccessToken_ShouldReturnCorrectValues()
        {
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token
                {
                    ClientId = "client1",
                    SubjectId = "subject1",
                    SessionId = "session1",
                    Description = "description1",
                    Scopes = new[] { "scope1", "scope2" }
                }
            };

            refreshToken.ClientId.Should().Be("client1");
            refreshToken.SubjectId.Should().Be("subject1");
            refreshToken.SessionId.Should().Be("session1");
            refreshToken.Description.Should().Be("description1");
            refreshToken.Scopes.Should().BeEquivalentTo(new[] { "scope1", "scope2" });
        }
    }
}
