using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class ExtensionGrantValidatorTests
    {
        private readonly ILogger<ExtensionGrantValidator> _logger;

        public ExtensionGrantValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<ExtensionGrantValidator>();
        }

        [Fact]
        public void GetAvailableGrantTypes_WhenNoValidators_ReturnsEmptyCollection()
        {
            var validator = new ExtensionGrantValidator(null, _logger);
            var types = validator.GetAvailableGrantTypes();
            types.Should().BeEmpty();
        }

        [Fact]
        public void GetAvailableGrantTypes_WhenValidatorsExist_ReturnsExpectedTypes()
        {
            var validators = new List<IExtensionGrantValidator>
            {
                new TestExtensionGrantValidator("grant1"),
                new TestExtensionGrantValidator("grant2")
            };

            var validator = new ExtensionGrantValidator(validators, _logger);
            var types = validator.GetAvailableGrantTypes();
            
            types.Should().BeEquivalentTo(new[] { "grant1", "grant2" });
        }

        [Fact]
        public async Task ValidateAsync_WhenValidatorNotFound_ReturnsUnsupportedGrantType()
        {
            var validator = new ExtensionGrantValidator(new[] { new TestExtensionGrantValidator("grant1") }, _logger);
            var request = new ValidatedTokenRequest { GrantType = "unknown" };

            var result = await validator.ValidateAsync(request);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(TokenRequestErrors.UnsupportedGrantType);
        }

        [Fact]
        public async Task ValidateAsync_WhenValidatorSucceeds_ReturnsSuccessResult()
        {
            var testValidator = new TestExtensionGrantValidator("grant1");
            var validator = new ExtensionGrantValidator(new[] { testValidator }, _logger);
            var request = new ValidatedTokenRequest { GrantType = "grant1" };

            var result = await validator.ValidateAsync(request);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateAsync_WhenValidatorThrows_ReturnsInvalidGrant()
        {
            var testValidator = new TestExtensionGrantValidator("grant1", throwError: true);
            var validator = new ExtensionGrantValidator(new[] { testValidator }, _logger);
            var request = new ValidatedTokenRequest { GrantType = "grant1" };

            var result = await validator.ValidateAsync(request);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(TokenRequestErrors.InvalidGrant);
        }

        private class TestExtensionGrantValidator : IExtensionGrantValidator
        {
            private readonly bool _throwError;

            public TestExtensionGrantValidator(string grantType, bool throwError = false)
            {
                GrantType = grantType;
                _throwError = throwError;
            }

            public string GrantType { get; }

            public Task ValidateAsync(ExtensionGrantValidationContext context)
            {
                if (_throwError)
                {
                    throw new Exception("Validation error");
                }

                context.Result = new GrantValidationResult("subject", "custom");
                return Task.CompletedTask;
            }
        }
    }
}
