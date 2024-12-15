using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;
using IdentityServer4.Models;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationCodeTests
    {
        [Fact]
        public void AuthorizationCode_DefaultConstructor_SetsDefaultValues()
        {
            // Act
            var code = new AuthorizationCode();

            // Assert
            Assert.NotEqual(default(DateTime), code.CreationTime);
            Assert.NotNull(code.Properties);
            Assert.Empty(code.Properties);
        }

        [Fact]
        public void AuthorizationCode_Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var code = new AuthorizationCode
            {
                ClientId = "test_client",
                CreationTime = new DateTime(2024, 1, 1),
                Lifetime = 300,
                IsOpenId = true,
                RedirectUri = "https://test.com/callback",
                Nonce = "test_nonce",
                StateHash = "test_state_hash",
                WasConsentShown = true,
                SessionId = "test_session",
                CodeChallenge = "test_challenge",
                CodeChallengeMethod = "S256",
                Description = "test_description"
            };

            var claims = new List<Claim>
            {
                new Claim("sub", "123")
            };
            code.Subject = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            code.RequestedScopes = new[] { "openid", "profile" };
            code.Properties.Add("custom_property", "custom_value");

            // Assert
            Assert.Equal("test_client", code.ClientId);
            Assert.Equal(new DateTime(2024, 1, 1), code.CreationTime);
            Assert.Equal(300, code.Lifetime);
            Assert.True(code.IsOpenId);
            Assert.Equal("https://test.com/callback", code.RedirectUri);
            Assert.Equal("test_nonce", code.Nonce);
            Assert.Equal("test_state_hash", code.StateHash);
            Assert.True(code.WasConsentShown);
            Assert.Equal("test_session", code.SessionId);
            Assert.Equal("test_challenge", code.CodeChallenge);
            Assert.Equal("S256", code.CodeChallengeMethod);
            Assert.Equal("test_description", code.Description);
            Assert.Equal("123", code.Subject.FindFirst("sub")?.Value);
            Assert.Contains("openid", code.RequestedScopes);
            Assert.Contains("profile", code.RequestedScopes);
            Assert.Equal("custom_value", code.Properties["custom_property"]);
        }

        [Fact]
        public void AuthorizationCode_Properties_DefaultsToEmptyDictionary()
        {
            // Arrange
            var code = new AuthorizationCode();

            // Assert
            Assert.NotNull(code.Properties);
            Assert.Empty(code.Properties);
        }
    }
}
