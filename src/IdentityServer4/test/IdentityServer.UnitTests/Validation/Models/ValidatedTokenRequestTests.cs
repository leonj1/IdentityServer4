using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class ValidatedTokenRequestTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateValidInstance()
        {
            // Act
            var request = new ValidatedTokenRequest();

            // Assert
            Assert.NotNull(request);
            Assert.Null(request.GrantType);
            Assert.Null(request.RequestedScopes);
            Assert.Null(request.UserName);
            Assert.Null(request.RefreshToken);
            Assert.Null(request.RefreshTokenHandle);
            Assert.Null(request.AuthorizationCode);
            Assert.Null(request.AuthorizationCodeHandle);
            Assert.Null(request.CodeVerifier);
            Assert.Null(request.DeviceCode);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var request = new ValidatedTokenRequest
            {
                GrantType = "authorization_code",
                RequestedScopes = new[] { "openid", "profile" },
                UserName = "testuser",
                RefreshToken = new RefreshToken(),
                RefreshTokenHandle = "refresh_token_handle",
                AuthorizationCode = new AuthorizationCode(),
                AuthorizationCodeHandle = "auth_code_handle",
                CodeVerifier = "code_verifier",
                DeviceCode = new DeviceCode()
            };

            // Assert
            Assert.Equal("authorization_code", request.GrantType);
            Assert.Equal(2, request.RequestedScopes.Count());
            Assert.Contains("openid", request.RequestedScopes);
            Assert.Contains("profile", request.RequestedScopes);
            Assert.Equal("testuser", request.UserName);
            Assert.NotNull(request.RefreshToken);
            Assert.Equal("refresh_token_handle", request.RefreshTokenHandle);
            Assert.NotNull(request.AuthorizationCode);
            Assert.Equal("auth_code_handle", request.AuthorizationCodeHandle);
            Assert.Equal("code_verifier", request.CodeVerifier);
            Assert.NotNull(request.DeviceCode);
        }

        [Fact]
        public void InheritsFrom_ValidatedRequest()
        {
            // Arrange
            var request = new ValidatedTokenRequest();

            // Assert
            Assert.IsAssignableFrom<ValidatedRequest>(request);
        }
    }
}
