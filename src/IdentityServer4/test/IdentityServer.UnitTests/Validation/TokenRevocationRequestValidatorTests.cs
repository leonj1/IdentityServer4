using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenRevocationRequestValidatorTests
    {
        private readonly TokenRevocationRequestValidator _validator;
        private readonly ILogger<TokenRevocationRequestValidator> _logger;

        public TokenRevocationRequestValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<TokenRevocationRequestValidator>();
            _validator = new TokenRevocationRequestValidator(_logger);
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenParametersNull_ThrowsArgumentNullException()
        {
            // Arrange
            var client = new Client();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestAsync(null, client));
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenClientNull_ThrowsArgumentNullException()
        {
            // Arrange
            var parameters = new NameValueCollection();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateRequestAsync(parameters, null));
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenTokenMissing_ReturnsError()
        {
            // Arrange
            var parameters = new NameValueCollection();
            var client = new Client();

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenValidRequest_ReturnsSuccess()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" }
            };
            var client = new Client();

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, client);

            // Assert
            result.IsError.Should().BeFalse();
            result.Token.Should().Be("valid_token");
            result.Client.Should().Be(client);
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenValidTokenTypeHint_SetsHintCorrectly()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" },
                { "token_type_hint", "access_token" }
            };
            var client = new Client();

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, client);

            // Assert
            result.IsError.Should().BeFalse();
            result.TokenTypeHint.Should().Be("access_token");
        }

        [Fact]
        public async Task ValidateRequestAsync_WhenInvalidTokenTypeHint_ReturnsError()
        {
            // Arrange
            var parameters = new NameValueCollection
            {
                { "token", "valid_token" },
                { "token_type_hint", "invalid_type" }
            };
            var client = new Client();

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, client);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("unsupported_token_type");
        }
    }
}
