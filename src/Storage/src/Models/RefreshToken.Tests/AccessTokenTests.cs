using System;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AccessTokenTests
    {
        [Fact]
        public void Subject_ShouldCreatePrincipalWithClaims()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token
                {
                    SubjectId = "test_subject",
                    Claims = new[] 
                    { 
                        new Claim("name", "John Doe"),
                        new Claim("email", "john@example.com")
                    }
                }
            };

            // Act
            var subject = refreshToken.Subject;

            // Assert
            Assert.NotNull(subject);
            Assert.True(subject.Claims.Any(c => c.Type == "name" && c.Value == "John Doe"));
            Assert.True(subject.Claims.Any(c => c.Type == "email" && c.Value == "john@example.com"));
        }
    }
}
