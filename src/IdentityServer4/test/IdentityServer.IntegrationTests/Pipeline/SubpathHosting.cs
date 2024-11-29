// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Pipeline
{
    public class SubpathHosting
    {
        private const string Category = "Subpath endpoint";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        private Client _client1;

        public SubpathHosting()
        {
            _mockPipeline.Clients.AddRange(new Client[] {
                _client1 = new Client
                {
                    ClientId = "client1",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    AllowedScopes = new List<string> { "openid", "profile" },
                    RedirectUris = new List<string> { "https://client1/callback" },
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
            
            _mockPipeline.Initialize("/subpath");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task anonymous_user_should_be_redirected_to_login_page()
        {
            var url = new RequestUrl("https://server/subpath/connect/authorize").CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.LoginWasCalled.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task invalid_subpath_should_return_404()
        {
            var url = new RequestUrl("https://server/wrongpath/connect/authorize").CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task authenticated_user_should_receive_identity_token()
        {
            _mockPipeline.Login("bob");

            var url = new RequestUrl("https://server/subpath/connect/authorize").CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid profile",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            var response = await _mockPipeline.BrowserClient.GetAsync(url);
            
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().Contain("id_token=");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task subpath_discovery_endpoint_should_be_available()
        {
            var response = await _mockPipeline.BrowserClient.GetAsync("https://server/subpath/.well-known/openid-configuration");
            
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var disco = await response.Content.ReadAsStringAsync();
            disco.Should().Contain("\"issuer\":\"https://server/subpath\"");
        }
    }
}
