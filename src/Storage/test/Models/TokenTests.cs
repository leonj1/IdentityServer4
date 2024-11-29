using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Xunit;

namespace IdentityServer4.Models
{
    public class TokenTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeCollections()
        {
            // Act
            var token = new Token();

            // Assert
            Assert.NotNull(token.AllowedSigningAlgorithms);
            Assert.NotNull(token.Audiences);
            Assert.NotNull(token.Claims);
            Assert.Empty(token.AllowedSigningAlgorithms);
            Assert.Empty(token.Audiences);
            Assert.Empty(token.Claims);
        }

        [Fact]
        public void Constructor_WithTokenType_ShouldSetType()
        {
            // Arrange
            var tokenType = "custom_type";

            // Act
            var token = new Token(tokenType);

            // Assert
            Assert.Equal(tokenType, token.Type);
        }

        [Fact]
        public void SubjectId_ShouldReturnCorrectValue()
        {
            // Arrange
            var token = new Token();
            var expectedSubjectId = "123";
            token.Claims.Add(new Claim(JwtClaimTypes.Subject, expectedSubjectId));

            // Act
            var subjectId = token.SubjectId;

            // Assert
            Assert.Equal(expectedSubjectId, subjectId);
        }

        [Fact]
        public void SessionId_ShouldReturnCorrectValue()
        {
            // Arrange
            var token = new Token();
            var expectedSessionId = "session123";
            token.Claims.Add(new Claim(JwtClaimTypes.SessionId, expectedSessionId));

            // Act
            var sessionId = token.SessionId;

            // Assert
            Assert.Equal(expectedSessionId, sessionId);
        }

        [Fact]
        public void Scopes_ShouldReturnAllScopeClaims()
        {
            // Arrange
            var token = new Token();
            var expectedScopes = new[] { "scope1", "scope2", "scope3" };
            foreach (var scope in expectedScopes)
            {
                token.Claims.Add(new Claim(JwtClaimTypes.Scope, scope));
            }

            // Act
            var scopes = token.Scopes.ToArray();

            // Assert
            Assert.Equal(expectedScopes.Length, scopes.Length);
            Assert.Equal(expectedScopes, scopes);
        }

        [Fact]
        public void Version_ShouldDefaultToFour()
        {
            // Arrange
            var token = new Token();

            // Assert
            Assert.Equal(4, token.Version);
        }

        [Fact]
        public void Type_ShouldDefaultToAccessToken()
        {
            // Arrange
            var token = new Token();

            // Assert
            Assert.Equal(OidcConstants.TokenTypes.AccessToken, token.Type);
        }
    }
}
