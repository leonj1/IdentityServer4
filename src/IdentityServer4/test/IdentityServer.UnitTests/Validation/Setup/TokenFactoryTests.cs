using System;
using System.Linq;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TokenFactoryTests
    {
        private readonly Client _testClient;

        public TokenFactoryTests()
        {
            _testClient = new Client
            {
                ClientId = "test_client",
                AccessTokenType = AccessTokenType.Reference
            };
        }

        [Fact]
        public void CreateAccessToken_ShouldCreateValidToken()
        {
            // Arrange
            var subjectId = "123";
            var lifetime = 3600;
            var scopes = new[] { "api1", "api2" };

            // Act
            var token = TokenFactory.CreateAccessToken(_testClient, subjectId, lifetime, scopes);

            // Assert
            token.Should().NotBeNull();
            token.Type.Should().Be(OidcConstants.TokenTypes.AccessToken);
            token.ClientId.Should().Be(_testClient.ClientId);
            token.Lifetime.Should().Be(lifetime);
            token.AccessTokenType.Should().Be(_testClient.AccessTokenType);
            token.Claims.Should().Contain(c => c.Type == "sub" && c.Value == subjectId);
            token.Claims.Where(c => c.Type == "scope").Select(c => c.Value)
                .Should().BeEquivalentTo(scopes);
        }

        [Fact]
        public void CreateAccessTokenLong_ShouldCreateTokenWithExtraClaims()
        {
            // Arrange
            var subjectId = "123";
            var lifetime = 3600;
            var claimCount = 5;
            var scopes = new[] { "api1" };

            // Act
            var token = TokenFactory.CreateAccessTokenLong(_testClient, subjectId, lifetime, claimCount, scopes);

            // Assert
            token.Should().NotBeNull();
            token.Claims.Count().Should().Be(claimCount + 3); // base claims (client_id, sub) + scopes + junk claims
            token.Claims.Count(c => c.Type == "junk").Should().Be(claimCount);
        }

        [Fact]
        public void CreateIdentityToken_ShouldCreateValidToken()
        {
            // Arrange
            var clientId = "test_client";
            var subjectId = "123";

            // Act
            var token = TokenFactory.CreateIdentityToken(clientId, subjectId);

            // Assert
            token.Should().NotBeNull();
            token.Type.Should().Be(OidcConstants.TokenTypes.IdentityToken);
            token.ClientId.Should().Be(clientId);
            token.Lifetime.Should().Be(600);
            token.Claims.Should().Contain(c => c.Type == "sub" && c.Value == subjectId);
            token.Audiences.Should().Contain(clientId);
        }

        [Fact]
        public void CreateIdentityTokenLong_ShouldCreateTokenWithExtraClaims()
        {
            // Arrange
            var clientId = "test_client";
            var subjectId = "123";
            var claimCount = 5;

            // Act
            var token = TokenFactory.CreateIdentityTokenLong(clientId, subjectId, claimCount);

            // Assert
            token.Should().NotBeNull();
            token.Claims.Count().Should().Be(claimCount + 1); // sub claim + junk claims
            token.Claims.Count(c => c.Type == "junk").Should().Be(claimCount);
        }
    }
}
