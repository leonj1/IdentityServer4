using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class ResourceOwnerClientTests
    {
        [Fact]
        public void GetPayload_WithValidToken_ShouldReturnExpectedDictionary()
        {
            // Arrange
            var expectedPayload = new Dictionary<string, object>
            {
                { "sub", "88421113" },
                { "iss", "https://idsvr4" },
                { "aud", "api" }
            };
            
            var tokenParts = new[]
            {
                "header",
                Base64Url.Encode(JsonConvert.SerializeObject(expectedPayload)),
                "signature"
            };
            
            var tokenResponse = new TokenResponse
            {
                AccessToken = string.Join(".", tokenParts)
            };

            // Act
            var payload = ResourceOwnerClient.GetPayload(tokenResponse);

            // Assert
            payload.Should().ContainKey("sub").WhoseValue.Should().Be("88421113");
            payload.Should().ContainKey("iss").WhoseValue.Should().Be("https://idsvr4");
            payload.Should().ContainKey("aud").WhoseValue.Should().Be("api");
        }

        [Fact]
        public void GetPayload_WithInvalidToken_ShouldThrowException()
        {
            // Arrange
            var tokenResponse = new TokenResponse
            {
                AccessToken = "invalid.token.format"
            };

            // Act & Assert
            Action act = () => ResourceOwnerClient.GetPayload(tokenResponse);
            act.Should().Throw<JsonReaderException>();
        }
    }
}
