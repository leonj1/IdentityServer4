using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class SecretsListValidatorTests
    {
        private readonly ISecretsListValidator _validator;
        private readonly List<Secret> _secrets;
        private readonly ParsedSecret _parsedSecret;

        public SecretsListValidatorTests()
        {
            _validator = new SecretsListValidator();
            _secrets = new List<Secret>
            {
                new Secret("secret1".Sha256()),
                new Secret("secret2".Sha256())
            };
            _parsedSecret = new ParsedSecret
            {
                Id = "test",
                Credential = "secret1"
            };
        }

        [Fact]
        public async Task ValidateAsync_WhenSecretsAreNull_ShouldReturnFailure()
        {
            // Arrange
            IEnumerable<Secret> secrets = null;

            // Act
            var result = await _validator.ValidateAsync(secrets, _parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenParsedSecretIsNull_ShouldReturnFailure()
        {
            // Act
            var result = await _validator.ValidateAsync(_secrets, null);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenValidSecretProvided_ShouldReturnSuccess()
        {
            // Act
            var result = await _validator.ValidateAsync(_secrets, _parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_WhenInvalidSecretProvided_ShouldReturnFailure()
        {
            // Arrange
            _parsedSecret.Credential = "invalid_secret";

            // Act
            var result = await _validator.ValidateAsync(_secrets, _parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }
    }
}
