using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Logging.Models;
using Xunit;

namespace IdentityServer.UnitTests.Logging.Models
{
    public class TokenValidationLogTests
    {
        [Fact]
        public void ToString_Should_Serialize_Complete_Object()
        {
            // Arrange
            var log = new TokenValidationLog
            {
                ClientId = "test_client",
                ClientName = "Test Client",
                ValidateLifetime = true,
                AccessTokenType = "JWT",
                ExpectedScope = "api1",
                TokenHandle = "handle123",
                JwtId = "jwt123",
                Claims = new Dictionary<string, object>
                {
                    { "sub", "123" },
                    { "name", "Test User" }
                }
            };

            // Act
            var result = log.ToString();

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain("test_client");
            result.Should().Contain("Test Client");
            result.Should().Contain("JWT");
            result.Should().Contain("api1");
            result.Should().Contain("handle123");
            result.Should().Contain("jwt123");
            result.Should().Contain("123");
            result.Should().Contain("Test User");
        }

        [Fact]
        public void ToString_Should_Handle_Null_Claims()
        {
            // Arrange
            var log = new TokenValidationLog
            {
                ClientId = "test_client",
                Claims = null
            };

            // Act
            var result = log.ToString();

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain("test_client");
        }
    }
}
