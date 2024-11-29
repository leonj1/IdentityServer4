using System.Collections.Specialized;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class AuthorizeResponseExtensionsTests
    {
        [Fact]
        public void ToNameValueCollection_WhenErrorResponse_ShouldContainErrorDetails()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                IsError = true,
                Error = "invalid_request",
                ErrorDescription = "Invalid request parameter",
                State = "123"
            };

            // Act
            var result = response.ToNameValueCollection();

            // Assert
            Assert.Equal("invalid_request", result["error"]);
            Assert.Equal("Invalid request parameter", result["error_description"]);
            Assert.Equal("123", result["state"]);
        }

        [Fact]
        public void ToNameValueCollection_WhenSuccessfulResponse_ShouldContainAuthorizationDetails()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                IsError = false,
                Code = "auth_code",
                IdentityToken = "id_token",
                AccessToken = "access_token",
                AccessTokenLifetime = 3600,
                Scope = "openid profile",
                State = "123",
                SessionState = "session_123"
            };

            // Act
            var result = response.ToNameValueCollection();

            // Assert
            Assert.Equal("auth_code", result["code"]);
            Assert.Equal("id_token", result["id_token"]);
            Assert.Equal("access_token", result["access_token"]);
            Assert.Equal("Bearer", result["token_type"]);
            Assert.Equal("3600", result["expires_in"]);
            Assert.Equal("openid profile", result["scope"]);
            Assert.Equal("123", result["state"]);
            Assert.Equal("session_123", result["session_state"]);
        }

        [Fact]
        public void ToNameValueCollection_WhenMinimalResponse_ShouldOnlyContainRequiredFields()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                IsError = false,
                Code = "auth_code"
            };

            // Act
            var result = response.ToNameValueCollection();

            // Assert
            Assert.Equal("auth_code", result["code"]);
            Assert.Null(result["state"]);
            Assert.Null(result["session_state"]);
            Assert.Null(result["id_token"]);
            Assert.Null(result["access_token"]);
        }
    }
}
