using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class CodeFlowTestsTests
    {
        private const string Category = "Conformance.Basic.CodeFlowTests.Tests";
        private IdentityServerPipeline _pipeline;

        public CodeFlowTestsTests()
        {
            _pipeline = new IdentityServerPipeline();
            SetupTestEnvironment();
        }

        private void SetupTestEnvironment()
        {
            _pipeline.IdentityScopes.Add(new IdentityResources.OpenId());
            _pipeline.Clients.Add(new Client
            {
                Enabled = true,
                ClientId = "code_pipeline.Client",
                ClientSecrets = new List<Secret> { new Secret("secret".Sha512()) },
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = { "openid" },
                RequireConsent = false,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    "https://code_pipeline.Client/callback",
                    "https://code_pipeline.Client/callback?foo=bar&baz=quux"
                }
            });

            _pipeline.Users.Add(new TestUser
            {
                SubjectId = "testuser",
                Username = "testuser",
                Claims = new Claim[]
                {
                    new Claim("name", "Test User"),
                    new Claim("email", "test@example.com"),
                    new Claim("role", "User")
                }
            });

            _pipeline.Initialize();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_RedirectUri_Should_Fail()
        {
            await _pipeline.LoginAsync("testuser");

            var nonce = Guid.NewGuid().ToString();
            _pipeline.BrowserClient.AllowAutoRedirect = false;

            var url = _pipeline.CreateAuthorizeUrl(
                clientId: "code_pipeline.Client",
                responseType: "code",
                scope: "openid",
                redirectUri: "https://invalid.url/callback",
                nonce: nonce);

            var response = await _pipeline.BrowserClient.GetAsync(url);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Scope_Should_Fail()
        {
            await _pipeline.LoginAsync("testuser");

            var nonce = Guid.NewGuid().ToString();
            _pipeline.BrowserClient.AllowAutoRedirect = false;

            var url = _pipeline.CreateAuthorizeUrl(
                clientId: "code_pipeline.Client",
                responseType: "code",
                scope: "invalid_scope",
                redirectUri: "https://code_pipeline.Client/callback",
                nonce: nonce);

            var response = await _pipeline.BrowserClient.GetAsync(url);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Request_With_Nonce_Should_Return_Nonce_In_IdToken()
        {
            await _pipeline.LoginAsync("testuser");

            var nonce = Guid.NewGuid().ToString();
            _pipeline.BrowserClient.AllowAutoRedirect = false;

            var url = _pipeline.CreateAuthorizeUrl(
                clientId: "code_pipeline.Client",
                responseType: "code",
                scope: "openid",
                redirectUri: "https://code_pipeline.Client/callback",
                nonce: nonce);

            var response = await _pipeline.BrowserClient.GetAsync(url);
            var authorization = _pipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            
            var tokenClient = new HttpClient(new MessageHandlerWrapper(_pipeline.Handler));
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",
                Code = authorization.Code,
                RedirectUri = "https://code_pipeline.Client/callback"
            });

            var token = new JwtSecurityToken(tokenResult.IdentityToken);
            token.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value.Should().Be(nonce);
        }
    }
}
