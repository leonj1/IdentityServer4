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
    public class RefreshTokenClientTests
    {
        private const string TokenEndpoint = "https://server/connect/token";
        private const string RevocationEndpoint = "https://server/connect/revocation";
        private readonly HttpClient _client;

        public RefreshTokenClientTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task Should_Fail_With_Invalid_Client_Secret()
        {
            // Arrange
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "api1 offline_access",
                UserName = "bob",
                Password = "bob"
            });

            // Act
            var refreshResponse = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient",
                ClientSecret = "wrong_secret",
                RefreshToken = response.RefreshToken
            });

            // Assert
            refreshResponse.IsError.Should().BeTrue();
            refreshResponse.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Should_Fail_With_Invalid_Refresh_Token()
        {
            // Act
            var response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient",
                ClientSecret = "secret",
                RefreshToken = "invalid_refresh_token"
            });

            // Assert
            response.IsError.Should().BeTrue();
            response.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Should_Fail_With_Missing_Refresh_Token()
        {
            // Act
            var response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient",
                ClientSecret = "secret",
                RefreshToken = null
            });

            // Assert
            response.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Return_Expected_Error_When_Revoking_Invalid_Token()
        {
            // Act
            var revocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = RevocationEndpoint,
                ClientId = "roclient",
                ClientSecret = "secret",
                Token = "invalid_token",
                TokenTypeHint = "refresh_token"
            });

            // Assert
            revocationResponse.IsError.Should().BeTrue();
        }
    }
}
