using System;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class RefreshTokenTests
    {
        [Fact]
        public void ConsumedTime_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var refreshToken = new RefreshToken();

            // Assert
            Assert.Null(refreshToken.ConsumedTime);
        }

        [Fact]
        public void AccessToken_Properties_ShouldPropagate()
        {
            // Arrange
            var accessToken = new Token
            {
                ClientId = "client123",
                SubjectId = "subject456",
                SessionId = "session789",
                Description = "Test Token",
                Scopes = new[] { "api1", "api2" }
            };

            var refreshToken = new RefreshToken
            {
                AccessToken = accessToken
            };

            // Assert
            Assert.Equal(accessToken.ClientId, refreshToken.ClientId);
            Assert.Equal(accessToken.SubjectId, refreshToken.SubjectId);
            Assert.Equal(accessToken.SessionId, refreshToken.SessionId);
            Assert.Equal(accessToken.Description, refreshToken.Description);
            Assert.Equal(accessToken.Scopes, refreshToken.Scopes);
        }
    }
}
