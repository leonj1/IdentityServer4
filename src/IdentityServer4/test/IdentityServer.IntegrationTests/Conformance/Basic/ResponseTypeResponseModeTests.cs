using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class ResponseTypeResponseModeTests
    {
        private const string Category = "Conformance.Basic.ResponseTypeResponseModeTests";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        public ResponseTypeResponseModeTests()
        {
            _mockPipeline.Initialize();
            _mockPipeline.BrowserClient.AllowAutoRedirect = false;
            _mockPipeline.Clients.Add(new Client
            {
                Enabled = true,
                ClientId = "code_client",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha512())
                },

                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = { "openid" },

                RequireConsent = false,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    "https://code_client/callback"
                }
            });

            _mockPipeline.IdentityScopes.Add(new IdentityResources.OpenId());

            _mockPipeline.Users.Add(new TestUser
            {
                SubjectId = "1",
                Username = "bob",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("name", "Bob Smith"),
                    new Claim("email", "bob.smith@idsrv.com")
                }
            });
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Request_missing_response_type_rejected()
        {
            await _mockPipeline.LoginAsync("bob");

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "code_client",
                responseType: null, // missing
                scope: "openid",
                redirectUri: "https://code_client/callback",
                state: state,
                nonce: nonce);

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorMessage.Error.Should().Be("unsupported_response_type");
        }
    }
}
