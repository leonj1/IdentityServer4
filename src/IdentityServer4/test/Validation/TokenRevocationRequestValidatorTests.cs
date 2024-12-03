using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class TokenRevocationRequestValidatorTests
    {
        private readonly Client _client;
        private readonly ITokenRevocationRequestValidator _validator;

        public TokenRevocationRequestValidatorTests()
        {
            _client = new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) }
            };
            
            _validator = new TokenRevocationRequestValidator();
        }

        [Fact]
        public async Task ValidRequest_ShouldSucceed()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" },
                { "token_type_hint", "access_token" }
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task MissingToken_ShouldFail()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token_type_hint", "access_token" }
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }

        [Fact]
        public async Task InvalidTokenTypeHint_ShouldFail()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" },
                { "token_type_hint", "invalid_type" }
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("unsupported_token_type");
        }

        [Fact]
        public async Task NullClient_ShouldFail()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" }
            };

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, null);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_client");
        }
    }
}
