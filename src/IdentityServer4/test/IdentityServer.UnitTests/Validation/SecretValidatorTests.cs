using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class SecretValidatorTests
    {
        private readonly ISystemClock _clock;
        private readonly Mock<ILogger<ISecretsListValidator>> _logger;
        
        public SecretValidatorTests()
        {
            _clock = new SystemClock();
            _logger = new Mock<ILogger<ISecretsListValidator>>();
        }

        [Fact]
        public async Task ValidateAsync_WhenNoValidators_ShouldReturnFalse()
        {
            // Arrange
            var validators = new List<ISecretValidator>();
            var secretValidator = new SecretValidator(_clock, validators, _logger.Object);
            var secrets = new[] { new Secret("test") };
            var parsedSecret = new ParsedSecret { Id = "test" };

            // Act
            var result = await secretValidator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenSecretExpired_ShouldNotValidate()
        {
            // Arrange
            var mockValidator = new Mock<ISecretValidator>();
            mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = true });

            var validators = new List<ISecretValidator> { mockValidator.Object };
            var secretValidator = new SecretValidator(_clock, validators, _logger.Object);
            
            var expiredSecret = new Secret("test")
            {
                Expiration = DateTime.UtcNow.AddDays(-1)
            };
            var secrets = new[] { expiredSecret };
            var parsedSecret = new ParsedSecret { Id = "test" };

            // Act
            var result = await secretValidator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenValidSecretAndValidator_ShouldReturnTrue()
        {
            // Arrange
            var mockValidator = new Mock<ISecretValidator>();
            mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = true });

            var validators = new List<ISecretValidator> { mockValidator.Object };
            var secretValidator = new SecretValidator(_clock, validators, _logger.Object);
            
            var validSecret = new Secret("test")
            {
                Expiration = DateTime.UtcNow.AddDays(1)
            };
            var secrets = new[] { validSecret };
            var parsedSecret = new ParsedSecret { Id = "test" };

            // Act
            var result = await secretValidator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_WhenMultipleValidators_ShouldTryAllUntilSuccess()
        {
            // Arrange
            var mockFailingValidator = new Mock<ISecretValidator>();
            mockFailingValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = false });

            var mockSuccessValidator = new Mock<ISecretValidator>();
            mockSuccessValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()))
                .ReturnsAsync(new SecretValidationResult { Success = true });

            var validators = new List<ISecretValidator> 
            { 
                mockFailingValidator.Object,
                mockSuccessValidator.Object
            };
            
            var secretValidator = new SecretValidator(_clock, validators, _logger.Object);
            var validSecret = new Secret("test")
            {
                Expiration = DateTime.UtcNow.AddDays(1)
            };
            var secrets = new[] { validSecret };
            var parsedSecret = new ParsedSecret { Id = "test" };

            // Act
            var result = await secretValidator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
            mockFailingValidator.Verify(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()), Times.Once);
            mockSuccessValidator.Verify(x => x.ValidateAsync(It.IsAny<IEnumerable<Secret>>(), It.IsAny<ParsedSecret>()), Times.Once);
        }
    }
}
