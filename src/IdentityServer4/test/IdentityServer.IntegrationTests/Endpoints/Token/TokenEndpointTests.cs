using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Token
{
    public class TokenEndpointTests
    {
        private const string Category = "Token endpoint";

        private string client_id = "client";
        private string client_secret = "secret";

        private string scope_name = "api";
        private string scope_secret = "api_secret";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        public TokenEndpointTests()
        {
            _mockPipeline.Clients.Add(new Client
            {
                ClientId = client_id,
                ClientSecrets = new List<Secret> { new Secret(client_secret.Sha256()) },
                AllowedGrantTypes = { GrantType.ClientCredentials, GrantType.ResourceOwnerPassword },
                AllowedScopes = new List<string> { "api" },
            });

            _mockPipeline.Users.Add(new TestUser
            {
                SubjectId = "1",
                Username = "bob",
                Password = "password"
            });

            _mockPipeline.IdentityScopes.AddRange(new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            });

            _mockPipeline.ApiResources.Add(new ApiResource("api", "My API"));

            _mockPipeline.ApiScopes.Add(new ApiScope(scope_name, "My Scope"));

            _mockPipeline.Initialize();
        }

        [Fact]
        public async Task ClientCredentialsGrantType_ShouldReturnAccessToken()
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope_name },
                { "client_id", client_id },
                { "client_secret", client_secret }
            };

            var form = new FormUrlEncodedContent(data);
            _mockPipeline.BackChannelClient.DefaultRequestHeaders.Add("Referer", "http://127.0.0.1:33086/appservice/appservice?t=1564165664142?load");
            var response = await _mockPipeline.BackChannelClient.PostAsync(IdentityServerPipeline.TokenEndpoint, form);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(json);
            result.ContainsKey("error").Should().BeFalse();
        }

        [Fact]
        public async Task ResourceOwnerPasswordCredentialsGrantType_ShouldReturnAccessToken()
        {
            var data = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", "bob" },
                { "password", "password" },
                { "client_id", client_id },
                { "client_secret", client_secret },
                { "scope", scope_name }
            };

            var form = new FormUrlEncodedContent(data);
            _mockPipeline.BackChannelClient.DefaultRequestHeaders.Add("Referer", "http://127.0.0.1:33086/appservice/appservice?t=1564165664142?load");
            var response = await _mockPipeline.BackChannelClient.PostAsync(IdentityServerPipeline.TokenEndpoint, form);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(json);
            result.ContainsKey("error").Should().BeFalse();
        }
    }
}
