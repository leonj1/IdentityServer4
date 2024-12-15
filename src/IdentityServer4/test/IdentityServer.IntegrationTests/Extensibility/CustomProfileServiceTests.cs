using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace CustomProfileServiceTests
{
    public class CustomProfileServiceTests
    {
        private IdentityServerPipeline _pipeline;

        public CustomProfileServiceTests()
        {
            _pipeline = new IdentityServerPipeline();
            _pipeline.OnPostConfigureServices = services =>
            {
                services.AddIdentityServer(options =>
                {
                    options.PublicOrigin = "https://idsrv";
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client
                    {
                        ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = { "api1" }
                    }
                })
                .AddInMemoryIdentityResources(new List<IdentityResource>
                {
                    new IdentityResources.ApiScope("api1")
                })
                .AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "alice",
                        Password = "password"
                    }
                })
                .AddDeveloperSigningCredential();
            };

            _pipeline.Clients.Add(new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" }
            });

            _pipeline.IdentityScopes.Add(new IdentityResource
            {
                Name = "api1",
                DisplayName = "API 1",
                UserClaims = { "name", "role" }
            });

            _pipeline.Users.Add(new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password"
            });
        }

        [Fact]
        public async Task Should_Authenticate_User()
        {
            await _pipeline.Initialize();
            await _pipeline.LoginAsync("alice", "password");

            var disco = await DiscoveryClient.GetAsync("https://idsrv");
            Assert.NotNull(disco);

            var tokenResponse = await TokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret"
            });

            Assert.NotNull(tokenResponse);
            Assert.True(tokenResponse.IsError);
        }
    }
}
