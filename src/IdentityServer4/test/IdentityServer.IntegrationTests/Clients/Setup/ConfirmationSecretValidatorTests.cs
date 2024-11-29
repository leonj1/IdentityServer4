using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class ConfirmationSecretValidatorTests
    {
        private readonly ConfirmationSecretValidator _validator;

        public ConfirmationSecretValidatorTests()
        {
            _validator = new ConfirmationSecretValidator();
        }

        [Fact]
        public async Task When_ValidConfirmationSecret_Expect_Success()
        {
            // Arrange
            var secrets = new[] 
            { 
                new Secret { Type = "confirmation.test" } 
            };
            var parsedSecret = new ParsedSecret();

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
            
            var confirmation = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Confirmation);
            confirmation.Should().ContainKey("x5t#S256");
            confirmation["x5t#S256"].Should().Be("foo");
        }

        [Fact]
        public async Task When_InvalidSecretType_Expect_Failure()
        {
            // Arrange
            var secrets = new[] 
            { 
                new Secret { Type = "invalid.type" } 
            };
            var parsedSecret = new ParsedSecret();

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task When_EmptySecrets_Expect_Failure()
        {
            // Arrange
            var secrets = new Secret[] { };
            var parsedSecret = new ParsedSecret();

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }
    }
}
