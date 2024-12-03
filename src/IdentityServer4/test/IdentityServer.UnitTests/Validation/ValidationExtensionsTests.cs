using IdentityServer4.Models;
using IdentityServer4.Validation;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ValidationExtensionsTests
    {
        [Fact]
        public void ToValidationResult_ShouldCreateValidResult_WithClientAndSecret()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };
            
            var secret = new ParsedSecret
            {
                Id = "secret_id",
                Credential = "credential"
            };

            // Act
            var result = client.ToValidationResult(secret);

            // Assert
            result.Should().NotBeNull();
            result.Client.Should().Be(client);
            result.Secret.Should().Be(secret);
        }

        [Fact]
        public void ToValidationResult_ShouldCreateValidResult_WithNullSecret()
        {
            // Arrange
            var client = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            // Act
            var result = client.ToValidationResult();

            // Assert
            result.Should().NotBeNull();
            result.Client.Should().Be(client);
            result.Secret.Should().BeNull();
        }
    }
}
