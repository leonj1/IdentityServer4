using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class AuthorizationCodeTests
    {
        [Fact]
        public void DefaultConstructor_PropertiesShouldBeInitialized()
        {
            var code = new AuthorizationCode();
            
            Assert.NotNull(code.Properties);
            Assert.Empty(code.Properties);
        }

        [Fact]
        public void SettingProperties_ShouldWork()
        {
            var code = new AuthorizationCode
            {
                ClientId = "client1",
                CreationTime = new DateTime(2024, 1, 1),
                Lifetime = 300,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "123") })),
                IsOpenId = true,
                RequestedScopes = new[] { "openid", "profile" },
                RedirectUri = "https://client1/callback",
                Nonce = "nonce123",
                StateHash = "statehash123",
                WasConsentShown = true,
                SessionId = "session123",
                CodeChallenge = "challenge123",
                CodeChallengeMethod = "S256",
                Description = "Test Device",
            };

            Assert.Equal("client1", code.ClientId);
            Assert.Equal(new DateTime(2024, 1, 1), code.CreationTime);
            Assert.Equal(300, code.Lifetime);
            Assert.NotNull(code.Subject);
            Assert.True(code.IsOpenId);
            Assert.Contains("openid", code.RequestedScopes);
            Assert.Contains("profile", code.RequestedScopes);
            Assert.Equal("https://client1/callback", code.RedirectUri);
            Assert.Equal("nonce123", code.Nonce);
            Assert.Equal("statehash123", code.StateHash);
            Assert.True(code.WasConsentShown);
            Assert.Equal("session123", code.SessionId);
            Assert.Equal("challenge123", code.CodeChallenge);
            Assert.Equal("S256", code.CodeChallengeMethod);
            Assert.Equal("Test Device", code.Description);
        }

        [Fact]
        public void Properties_AddingAndRetrieving_ShouldWork()
        {
            var code = new AuthorizationCode();
            
            code.Properties.Add("key1", "value1");
            code.Properties.Add("key2", "value2");

            Assert.Equal(2, code.Properties.Count);
            Assert.Equal("value1", code.Properties["key1"]);
            Assert.Equal("value2", code.Properties["key2"]);
        }
    }
}
