using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class ExtensionGrantClientTests
    {
        private readonly ExtensionGrantClient _client;

        public ExtensionGrantClientTests()
        {
            _client = new ExtensionGrantClient();
        }

        [Fact]
        public async Task Invalid_Credential_Should_Fail()
        {
            var response = await _client._client.RequestTokenAsync(new TokenRequest
            {
                Address = "https://server/connect/token",
                GrantType = "custom",
                ClientId = "client.custom",
                ClientSecret = "secret",
                Parameters =
                {
                    { "custom_credential", "" }, // Empty credential
                    { "scope", "api1" }
                }
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Invalid_Client_Secret_Should_Fail()
        {
            var response = await _client._client.RequestTokenAsync(new TokenRequest
            {
                Address = "https://server/connect/token",
                GrantType = "custom",
                ClientId = "client.custom",
                ClientSecret = "wrong_secret",
                Parameters =
                {
                    { "custom_credential", "custom credential" },
                    { "scope", "api1" }
                }
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Invalid_Scope_Should_Fail()
        {
            var response = await _client._client.RequestTokenAsync(new TokenRequest
            {
                Address = "https://server/connect/token",
                GrantType = "custom",
                ClientId = "client.custom",
                ClientSecret = "secret",
                Parameters =
                {
                    { "custom_credential", "custom credential" },
                    { "scope", "invalid_scope" }
                }
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Error.Should().Be("invalid_scope");
        }

        [Fact]
        public async Task Missing_Parameters_Should_Fail()
        {
            var response = await _client._client.RequestTokenAsync(new TokenRequest
            {
                Address = "https://server/connect/token",
                GrantType = "custom",
                ClientId = "client.custom",
                ClientSecret = "secret"
                // No parameters
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Error.Should().Be("invalid_grant");
        }
    }
}
