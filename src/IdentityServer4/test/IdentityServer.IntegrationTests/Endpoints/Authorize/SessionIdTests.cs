using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Authorize
{
    public class SessionIdTests
    {
        private const string Category = "SessionIdTests";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        public SessionIdTests()
        {
            _mockPipeline.Clients.AddRange(new Client[] {
                new Client
                {
                    ClientId = "client1",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    AllowedScopes = new List<string> { "openid", "profile" },
                    RedirectUris = new List<string> { "https://client1/callback" },
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "client2",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = true,
                    AllowedScopes = new List<string> { "openid", "profile", "api1", "api2" },
                    RedirectUris = new List<string> { "https://client2/callback" },
                    AllowAccessTokensViaBrowser = true
                }
            });

            _mockPipeline.Users.Add(new TestUser
            {
                SubjectId = "bob",
                Username = "bob",
                Claims = new Claim[]
                {
                    new Claim("name", "Bob Loblaw"),
                    new Claim("email", "bob@loblaw.com"),
                    new Claim("role", "Attorney")
                }
            });

            _mockPipeline.IdentityScopes.AddRange(new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            });
            _mockPipeline.ApiResources.AddRange(new ApiResource[] {
                new ApiResource
                {
                    Name = "api",
                }
            });
            _mockPipeline.ApiScopes.AddRange(new ApiScope[] {
                new ApiScope("api1"),
                new ApiScope("api2")
            });

            _mockPipeline.Initialize();
        }

        [Fact]
        public async Task Should_not_change_session_id_after_login()
        {
            await _mockPipeline.LoginAsync("bob");

            var sidCookie = _mockPipeline.GetSessionCookieValue();

            // Simulate a request to the discovery endpoint
            // ...

            var newSidCookie = _mockPipeline.GetSessionCookieValue();
            newSidCookie.Should().Be(sidCookie);
        }
    }
}
