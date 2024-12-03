using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.ResponseHandling;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class TokenResponseTests
    {
        [Fact]
        public void TokenResponse_DefaultConstructor_ShouldInitializeCustomDictionary()
        {
            // Arrange & Act
            var response = new TokenResponse();

            // Assert
            response.Custom.Should().NotBeNull();
            response.Custom.Should().BeEmpty();
        }

        [Fact]
        public void TokenResponse_Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var response = new TokenResponse
            {
                IdentityToken = "id_token",
                AccessToken = "access_token",
                AccessTokenLifetime = 3600,
                RefreshToken = "refresh_token",
                Scope = "openid profile"
            };

            // Act & Assert
            response.IdentityToken.Should().Be("id_token");
            response.AccessToken.Should().Be("access_token");
            response.AccessTokenLifetime.Should().Be(3600);
            response.RefreshToken.Should().Be("refresh_token");
            response.Scope.Should().Be("openid profile");
        }

        [Fact]
        public void TokenResponse_CustomDictionary_ShouldAllowAddingCustomValues()
        {
            // Arrange
            var response = new TokenResponse();
            
            // Act
            response.Custom.Add("custom_key", "custom_value");

            // Assert
            response.Custom.Should().ContainKey("custom_key");
            response.Custom["custom_key"].Should().Be("custom_value");
        }
    }
}
