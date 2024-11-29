using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Events
{
    public class TokenIssuedSuccessEventTests
    {
        [Fact]
        public void TokenIssuedSuccessEvent_Constructor_WithAuthorizeResponse_ShouldSetProperties()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "client123",
                ClientName = "Test Client"
            };

            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            var request = new ValidatedAuthorizeRequest
            {
                ClientId = client.ClientId,
                Client = client,
                Subject = subject,
                GrantType = OidcConstants.GrantTypes.AuthorizationCode
            };

            var response = new AuthorizeResponse
            {
                Request = request,
                RedirectUri = "https://client.com/callback",
                IdentityToken = "id_token",
                Code = "auth_code",
                AccessToken = "access_token",
                Scope = "openid profile"
            };

            // Act
            var evt = new TokenIssuedSuccessEvent(response);

            // Assert
            evt.ClientId.Should().Be("client123");
            evt.ClientName.Should().Be("Test Client");
            evt.RedirectUri.Should().Be("https://client.com/callback");
            evt.Endpoint.Should().Be(Constants.EndpointNames.Authorize);
            evt.SubjectId.Should().Be("123");
            evt.Scopes.Should().Be("openid profile");
            evt.GrantType.Should().Be(OidcConstants.GrantTypes.AuthorizationCode);

            evt.Tokens.Count().Should().Be(3);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.TokenTypes.IdentityToken);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.ResponseTypes.Code);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.TokenTypes.AccessToken);
        }

        [Fact]
        public void TokenIssuedSuccessEvent_Constructor_WithTokenResponse_ShouldSetProperties()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "client123",
                ClientName = "Test Client"
            };

            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            var validatedRequest = new ValidatedTokenRequest
            {
                Client = client,
                Subject = subject,
                GrantType = OidcConstants.GrantTypes.ClientCredentials
            };

            var validationResult = new TokenRequestValidationResult(validatedRequest);
            validationResult.ValidatedRequest.ValidatedResources = new ResourceValidationResult();
            validationResult.ValidatedRequest.ValidatedResources.RawScopeValues = new[] { "api1", "api2" };

            var response = new TokenResponse
            {
                IdentityToken = "id_token",
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            };

            // Act
            var evt = new TokenIssuedSuccessEvent(response, validationResult);

            // Assert
            evt.ClientId.Should().Be("client123");
            evt.ClientName.Should().Be("Test Client");
            evt.Endpoint.Should().Be(Constants.EndpointNames.Token);
            evt.SubjectId.Should().Be("123");
            evt.Scopes.Should().Be("api1 api2");
            evt.GrantType.Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            evt.Tokens.Count().Should().Be(3);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.TokenTypes.IdentityToken);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.TokenTypes.AccessToken);
            evt.Tokens.Should().Contain(t => t.TokenType == OidcConstants.TokenTypes.RefreshToken);
        }
    }
}
