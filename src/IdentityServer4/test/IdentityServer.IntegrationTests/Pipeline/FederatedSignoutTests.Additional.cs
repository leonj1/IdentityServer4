using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.IntegrationTests.Pipeline
{
    public class FederatedSignoutAdditionalTests
    {
        private const string Category = "Federated Signout Additional";

        private IdentityServerPipeline _pipeline = new IdentityServerPipeline();
        private ClaimsPrincipal _user;

        public FederatedSignoutAdditionalTests()
        {
            _user = new IdentityServerUser("bob")
            {
                AdditionalClaims = { new Claim(JwtClaimTypes.SessionId, "123") }
            }.CreatePrincipal();

            _pipeline = new IdentityServerPipeline();

            _pipeline.IdentityScopes.AddRange(new IdentityResource[] {
                new IdentityResources.OpenId()
            });

            _pipeline.Clients.Add(new Client
            {
                ClientId = "client1",
                AllowedGrantTypes = GrantTypes.Implicit,
                RequireConsent = false,
                AllowedScopes = new List<string> { "openid" },
                RedirectUris = new List<string> { "https://client1/callback" },
                FrontChannelLogoutUri = "https://client1/signout",
                PostLogoutRedirectUris = new List<string> { "https://client1/signout-callback" },
                AllowAccessTokensViaBrowser = true
            });

            _pipeline.Users.Add(new TestUser
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

            _pipeline.Initialize();
        }

        [Fact]
        public async Task Invalid_Sid_Should_Return_Empty_Response()
        {
            await _pipeline.LoginAsync(_user);

            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=invalid_sid");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task Missing_Sid_Parameter_Should_Return_Empty_Response()
        {
            await _pipeline.LoginAsync(_user);

            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task Post_Request_With_Invalid_Sid_Should_Return_Empty_Response()
        {
            await _pipeline.LoginAsync(_user);

            var response = await _pipeline.BrowserClient.PostAsync(
                IdentityServerPipeline.FederatedSignOutUrl, 
                new FormUrlEncodedContent(new Dictionary<string, string> { { "sid", "invalid_sid" } }));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task Multiple_Concurrent_Signout_Requests_Should_Be_Handled()
        {
            await _pipeline.LoginAsync(_user);

            await _pipeline.RequestAuthorizationEndpointAsync(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            var tasks = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123"));
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var response in responses)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Content.Headers.ContentType.MediaType.Should().Be("text/html");
                var html = await response.Content.ReadAsStringAsync();
                html.Should().Contain("https://server/connect/endsession/callback?endSessionId=");
            }
        }
    }
}
