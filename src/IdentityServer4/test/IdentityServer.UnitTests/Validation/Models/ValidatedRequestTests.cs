using System;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Models
{
    public class ValidatedRequestTests
    {
        [Fact]
        public void SetClient_WhenValidClient_ShouldSetProperties()
        {
            // Arrange
            var validatedRequest = new ValidatedRequest();
            var client = new Client
            {
                ClientId = "test_client",
                AccessTokenLifetime = 3600,
                AccessTokenType = AccessTokenType.Reference,
                Claims = { new ClientClaim("test_type", "test_value") }
            };

            // Act
            validatedRequest.SetClient(client);

            // Assert
            validatedRequest.Client.Should().Be(client);
            validatedRequest.ClientId.Should().Be("test_client");
            validatedRequest.AccessTokenLifetime.Should().Be(3600);
            validatedRequest.AccessTokenType.Should().Be(AccessTokenType.Reference);
            validatedRequest.ClientClaims.Should().HaveCount(1);
            validatedRequest.ClientClaims.First().Type.Should().Be("test_type");
            validatedRequest.ClientClaims.First().Value.Should().Be("test_value");
        }

        [Fact]
        public void SetClient_WhenClientIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var validatedRequest = new ValidatedRequest();

            // Act & Assert
            Action act = () => validatedRequest.SetClient(null);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }

        [Fact]
        public void SetClient_WithSecretAndConfirmation_ShouldSetAdditionalProperties()
        {
            // Arrange
            var validatedRequest = new ValidatedRequest();
            var client = new Client { ClientId = "test_client" };
            var secret = new ParsedSecret { Id = "secret_id" };
            var confirmation = "test_confirmation";

            // Act
            validatedRequest.SetClient(client, secret, confirmation);

            // Assert
            validatedRequest.Secret.Should().Be(secret);
            validatedRequest.Confirmation.Should().Be(confirmation);
        }
    }
}
