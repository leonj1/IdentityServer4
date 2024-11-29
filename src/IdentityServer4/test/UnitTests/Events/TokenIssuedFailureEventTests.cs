using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer4.UnitTests.Events
{
    public class TokenIssuedFailureEventTests
    {
        [Fact]
        public void TokenIssuedFailureEvent_Constructor_WithValidatedAuthorizeRequest_ShouldSetProperties()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }, "test"));

            var client = new Client
            {
                ClientId = "client",
                ClientName = "Test Client"
            };

            var request = new ValidatedAuthorizeRequest
            {
                Subject = subject,
                Client = client,
                ClientId = client.ClientId,
                RedirectUri = "https://test.com",
                RequestedScopes = new[] { "openid", "profile" },
                GrantType = "authorization_code"
            };

            // Act
            var evt = new TokenIssuedFailureEvent(request, "invalid_request", "Invalid request description");

            // Assert
            evt.ClientId.Should().Be("client");
            evt.ClientName.Should().Be("Test Client");
            evt.RedirectUri.Should().Be("https://test.com");
            evt.Scopes.Should().Be("openid profile");
            evt.GrantType.Should().Be("authorization_code");
            evt.SubjectId.Should().Be("123");
            evt.Error.Should().Be("invalid_request");
            evt.ErrorDescription.Should().Be("Invalid request description");
        }

        [Fact]
        public void TokenIssuedFailureEvent_Constructor_WithTokenRequestValidationResult_ShouldSetProperties()
        {
            // Arrange
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }, "test"));

            var client = new Client
            {
                ClientId = "client",
                ClientName = "Test Client"
            };

            var validatedRequest = new ValidatedTokenRequest
            {
                Subject = subject,
                Client = client,
                GrantType = "client_credentials",
                RequestedScopes = new[] { "api1", "api2" }
            };

            var result = new TokenRequestValidationResult(validatedRequest)
            {
                Error = "invalid_grant",
                ErrorDescription = "Invalid grant description"
            };

            // Act
            var evt = new TokenIssuedFailureEvent(result);

            // Assert
            evt.ClientId.Should().Be("client");
            evt.ClientName.Should().Be("Test Client");
            evt.Scopes.Should().Be("api1 api2");
            evt.GrantType.Should().Be("client_credentials");
            evt.SubjectId.Should().Be("123");
            evt.Error.Should().Be("invalid_grant");
            evt.ErrorDescription.Should().Be("Invalid grant description");
        }

        [Fact]
        public void TokenIssuedFailureEvent_Constructor_WithNullRequest_ShouldNotThrow()
        {
            // Act
            var evt = new TokenIssuedFailureEvent(request: null, error: "error", description: "description");

            // Assert
            evt.Error.Should().Be("error");
            evt.ErrorDescription.Should().Be("description");
            evt.ClientId.Should().BeNull();
            evt.ClientName.Should().BeNull();
            evt.RedirectUri.Should().BeNull();
            evt.SubjectId.Should().BeNull();
            evt.Scopes.Should().BeNull();
            evt.GrantType.Should().BeNull();
        }
    }
}
