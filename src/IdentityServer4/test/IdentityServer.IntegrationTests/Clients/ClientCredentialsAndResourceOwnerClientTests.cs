using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Clients.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class ClientCredentialsAndResourceOwnerClientTests
    {
        private const string TokenEndpoint = "https://server/connect/token";
        private readonly HttpClient _client;

        public ClientCredentialsAndResourceOwnerClientTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task Invalid_Client_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "invalid_client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            response.IsError.Should().Be(true);
            response.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Invalid_Client_Secret_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client.and.ro",
                ClientSecret = "invalid_secret",
                Scope = "api1"
            });

            response.IsError.Should().Be(true);
            response.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Invalid_Scope_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client.and.ro",
                ClientSecret = "secret",
                Scope = "invalid_scope"
            });

            response.IsError.Should().Be(true);
            response.Error.Should().Be("invalid_scope");
        }

        [Fact]
        public async Task Invalid_Password_Should_Return_Error()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client.and.ro",
                ClientSecret = "secret",
                Scope = "openid",
                UserName = "bob",
                Password = "wrong_password"
            });

            response.IsError.Should().Be(true);
            response.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Invalid_Username_Should_Return_Error()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client.and.ro",
                ClientSecret = "secret",
                Scope = "openid",
                UserName = "invalid_user",
                Password = "bob"
            });

            response.IsError.Should().Be(true);
            response.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Empty_Scope_Should_Return_Error()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "client.and.ro",
                ClientSecret = "secret",
                Scope = ""
            });

            response.IsError.Should().Be(true);
        }
    }
}
