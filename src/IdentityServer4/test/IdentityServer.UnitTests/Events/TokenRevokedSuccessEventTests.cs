using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class TokenRevokedSuccessEventTests
    {
        [Fact]
        public void TokenRevokedSuccessEvent_Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            var requestResult = new TokenRevocationRequestValidationResult
            {
                Token = "some_token",
                TokenTypeHint = "access_token"
            };

            // Act
            var evt = new TokenRevokedSuccessEvent(requestResult, client);

            // Assert
            evt.ClientId.Should().Be("test_client");
            evt.ClientName.Should().Be("Test Client");
            evt.TokenType.Should().Be("access_token");
            evt.Token.Should().NotBe("some_token"); // Token should be obfuscated
            evt.Category.Should().Be(EventCategories.Token);
            evt.Name.Should().Be("Token Revoked Success");
            evt.EventType.Should().Be(EventTypes.Success);
            evt.Id.Should().Be(EventIds.TokenRevokedSuccess);
        }
    }
}
