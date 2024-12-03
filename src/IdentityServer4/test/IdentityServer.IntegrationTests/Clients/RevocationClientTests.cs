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
    public class RevocationClientTests
    {
        private const string TokenEndpoint = "https://server/connect/token";
        private const string RevocationEndpoint = "https://server/connect/revocation";
        private const string IntrospectionEndpoint = "https://server/connect/introspect";

        private readonly HttpClient _client;

        public RevocationClientTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _client = server.CreateClient();
        }

        [Fact]
        public async Task Revoking_Invalid_Token_Should_Return_Success()
        {
            var revocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = RevocationEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "secret",
                Token = "invalid_token"
            });

            revocationResponse.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Revoking_Token_With_Invalid_Client_Credentials_Should_Fail()
        {
            var revocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = RevocationEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "invalid_secret",
                Token = "some_token"
            });

            revocationResponse.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task Revoking_Refresh_Token_Should_Also_Invalidate_Access_Token()
        {
            // Request initial token
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "secret",
                Scope = "api1 offline_access",
                UserName = "bob",
                Password = "bob"
            });

            response.IsError.Should().BeFalse();
            response.RefreshToken.Should().NotBeNullOrEmpty();

            // Verify access token is active
            var introspectionResponse = await _client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = IntrospectionEndpoint,
                ClientId = "api",
                ClientSecret = "secret",
                Token = response.AccessToken
            });

            introspectionResponse.IsActive.Should().BeTrue();

            // Revoke refresh token
            var revocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = RevocationEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "secret",
                Token = response.RefreshToken
            });

            revocationResponse.IsError.Should().BeFalse();

            // Verify access token is now inactive
            introspectionResponse = await _client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = IntrospectionEndpoint,
                ClientId = "api",
                ClientSecret = "secret",
                Token = response.AccessToken
            });

            introspectionResponse.IsActive.Should().BeFalse();
        }
    }
}
