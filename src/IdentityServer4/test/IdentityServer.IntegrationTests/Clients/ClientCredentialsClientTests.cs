using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class ClientCredentialsClientTests : ClientCredentialsClient
    {
        [Fact]
        public async Task Empty_Client_Id_Should_Fail()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "",
                ClientSecret = "secret",
                Scope = "api1"
            });

            response.IsError.Should().Be(true);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Null_Client_Id_Should_Fail()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = null,
                ClientSecret = "secret",
                Scope = "api1"
            });

            response.IsError.Should().Be(true);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Empty_Scope_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = ""
            });

            response.IsError.Should().Be(true);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Error.Should().Be("invalid_scope");
        }

        [Fact]
        public async Task Invalid_Token_Endpoint_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://invalid-server/connect/token",
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            response.IsError.Should().Be(true);
            response.ErrorType.Should().Be(ResponseErrorType.Http);
        }

        [Fact]
        public async Task Multiple_Invalid_Scopes_Should_Fail()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "invalid1 invalid2 invalid3"
            });

            response.IsError.Should().Be(true);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("invalid_scope");
        }
    }
}
