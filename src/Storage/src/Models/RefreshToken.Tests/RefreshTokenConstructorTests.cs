using System;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class RefreshTokenConstructorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var refreshToken = new RefreshToken
            {
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600,
                AccessToken = new Token
                {
                    ClientId = "test_client",
                    SubjectId = "test_subject",
                    SessionId = "test_session",
                    Description = "test_description",
                    Scopes = new[] { "scope1", "scope2" },
                    Claims = new[] { new Claim("test_type", "test_value") }
                }
            };

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(4, refreshToken.Version);
            Assert.Equal("test_client", refreshToken.ClientId);
            Assert.Equal("test_subject", refreshToken.SubjectId);
            Assert.Equal("test_session", refreshToken.SessionId);
            Assert.Equal("test_description", refreshToken.Description);
            Assert.Equal(2, refreshToken.Scopes.Count());
            Assert.Contains("scope1", refreshToken.Scopes);
            Assert.Contains("scope2", refreshToken.Scopes);
        }
    }
}
