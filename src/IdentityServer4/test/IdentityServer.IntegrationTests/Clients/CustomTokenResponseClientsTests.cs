using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IdentityServer.IntegrationTests.Clients
{
    public class CustomTokenResponseClientsTests
    {
        private readonly CustomTokenResponseClients _sut;

        public CustomTokenResponseClientsTests()
        {
            _sut = new CustomTokenResponseClients();
        }

        [Fact]
        public async Task GetFields_ShouldReturnExpectedDictionary()
        {
            // Arrange
            var tokenResponse = new TokenResponse(new JObject
            {
                ["string_value"] = "test_string",
                ["int_value"] = 42
            });

            // Act
            var result = _sut.GetType()
                .GetMethod("GetFields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_sut, new object[] { tokenResponse }) as System.Collections.Generic.Dictionary<string, object>;

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("string_value");
            result.Should().ContainKey("int_value");
            result["string_value"].Should().Be("test_string");
            ((Int64)result["int_value"]).Should().Be(42);
        }

        [Fact]
        public async Task GetDto_ShouldReturnExpectedCustomResponseDto()
        {
            // Arrange
            var jObject = JObject.FromObject(new
            {
                string_value = "test_string",
                int_value = 42,
                nested = new
                {
                    string_value = "nested_string",
                    int_value = 24
                }
            });

            // Act
            var result = _sut.GetType()
                .GetMethod("GetDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_sut, new object[] { jObject }) as CustomResponseDto;

            // Assert
            result.Should().NotBeNull();
            result.string_value.Should().Be("test_string");
            result.int_value.Should().Be(42);
            result.nested.Should().NotBeNull();
            result.nested.string_value.Should().Be("nested_string");
            result.nested.int_value.Should().Be(24);
        }

        [Fact]
        public async Task GetPayload_ShouldReturnExpectedDictionary()
        {
            // Arrange
            var payload = new JObject
            {
                ["iss"] = "https://idsvr4",
                ["sub"] = "test_subject",
                ["aud"] = "api"
            };
            
            var encodedPayload = Base64UrlEncoder.Encode(payload.ToString());
            var tokenResponse = new TokenResponse(new JObject
            {
                ["access_token"] = $"header.{encodedPayload}.signature"
            });

            // Act
            var result = _sut.GetType()
                .GetMethod("GetPayload", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_sut, new object[] { tokenResponse }) as System.Collections.Generic.Dictionary<string, object>;

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("iss");
            result.Should().ContainKey("sub");
            result.Should().ContainKey("aud");
            result["iss"].Should().Be("https://idsvr4");
            result["sub"].Should().Be("test_subject");
            result["aud"].Should().Be("api");
        }
    }
}
