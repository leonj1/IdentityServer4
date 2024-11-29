using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.ResponseHandling
{
    public class AuthorizeResponseTests
    {
        [Fact]
        public void IsError_Should_ReturnTrue_When_ErrorIsPresent()
        {
            // Arrange
            var response = new AuthorizeResponse
            {
                Error = "invalid_request"
            };

            // Act & Assert
            response.IsError.Should().BeTrue();
        }

        [Fact]
        public void IsError_Should_ReturnFalse_When_ErrorIsNotPresent()
        {
            // Arrange
            var response = new AuthorizeResponse();

            // Act & Assert
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public void Properties_Should_ReturnExpectedValues_When_RequestIsSet()
        {
            // Arrange
            var validatedRequest = new ValidatedAuthorizeRequest
            {
                RedirectUri = "https://client.com/callback",
                State = "abc123",
                ValidatedResources = new ResourceValidationResult
                {
                    RawScopeValues = new[] { "openid", "profile" }
                }
            };

            var response = new AuthorizeResponse
            {
                Request = validatedRequest,
                AccessToken = "access_token",
                IdentityToken = "id_token",
                Code = "authorization_code",
                SessionState = "session_state",
                AccessTokenLifetime = 3600
            };

            // Act & Assert
            response.RedirectUri.Should().Be("https://client.com/callback");
            response.State.Should().Be("abc123");
            response.Scope.Should().Be("openid profile");
            response.AccessToken.Should().Be("access_token");
            response.IdentityToken.Should().Be("id_token");
            response.Code.Should().Be("authorization_code");
            response.SessionState.Should().Be("session_state");
            response.AccessTokenLifetime.Should().Be(3600);
        }

        [Fact]
        public void Properties_Should_ReturnNull_When_RequestIsNull()
        {
            // Arrange
            var response = new AuthorizeResponse();

            // Act & Assert
            response.RedirectUri.Should().BeNull();
            response.State.Should().BeNull();
            response.Scope.Should().BeNull();
        }
    }
}
