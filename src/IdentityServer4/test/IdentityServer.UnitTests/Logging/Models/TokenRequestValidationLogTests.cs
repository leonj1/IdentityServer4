using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Logging.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Logging.Models
{
    public class TokenRequestValidationLogTests
    {
        [Fact]
        public void Should_Serialize_Token_Request_With_All_Properties()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            var validatedRequest = new ValidatedTokenRequest
            {
                Client = client,
                Raw = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "scope", "openid profile" },
                    { "password", "secret" }
                },
                GrantType = "authorization_code",
                AuthorizationCodeHandle = "authorization_code_value",
                RefreshTokenHandle = "refresh_token_value",
                UserName = "test_user",
                RequestedScopes = new[] { "openid", "profile" }
            };

            var sensitiveValues = new[] { "password" };

            // Act
            var log = new TokenRequestValidationLog(validatedRequest, sensitiveValues);

            // Assert
            log.ClientId.Should().Be("test_client");
            log.ClientName.Should().Be("Test Client");
            log.GrantType.Should().Be("authorization_code");
            log.Scopes.Should().Be("openid profile");
            log.AuthorizationCode.Should().NotBe("authorization_code_value");
            log.RefreshToken.Should().NotBe("refresh_token_value");
            log.UserName.Should().Be("test_user");
            
            // Verify sensitive data is scrubbed
            log.Raw.Should().ContainKey("password");
            log.Raw["password"].Should().Be("***REDACTED***");
        }
    }
}
