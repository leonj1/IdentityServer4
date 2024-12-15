// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityServer.IntegrationTests
{
    public class CodeFlowTests
    {
        private readonly IdentityServerPipeline _pipeline = new IdentityServerPipeline();

        [Fact]
        public async Task NoState_NoSHash()
        {
            await _pipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();

            _pipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _pipeline.CreateAuthorizeUrl(
                           clientId: "code_pipeline.Client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_pipeline.Client/callback?foo=bar&baz=quux",
                           nonce: nonce);
            var response = await _pipeline.BrowserClient.GetAsync(url);

            var authorization = _pipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client
            var wrapper = new MessageHandlerWrapper(_pipeline.BrowserClient);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",

                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback?foo=bar&baz=quux"
            });

            tokenResult.IsError.Should().BeFalse();
            tokenResult.HttpErrorReason.Should().Be("OK");
            tokenResult.TokenType.Should().Be("Bearer");
            tokenResult.AccessToken.Should().NotBeNull();
            tokenResult.ExpiresIn.Should().BeGreaterThan(0);
            tokenResult.IdentityToken.Should().NotBeNull();

            var token = new JwtSecurityToken(tokenResult.IdentityToken);

            var s_hash = token.Claims.FirstOrDefault(c => c.Type == "s_hash");
            s_hash.Should().BeNull();
        }

        [Fact]
        public async Task WithState_SHash()
        {
            await _pipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();

            _pipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _pipeline.CreateAuthorizeUrl(
                           clientId: "code_pipeline.Client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_pipeline.Client/callback?foo=bar&baz=quux",
                           state: "state",
                           nonce: nonce);
            var response = await _pipeline.BrowserClient.GetAsync(url);

            var authorization = _pipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client
            var wrapper = new MessageHandlerWrapper(_pipeline.BrowserClient);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",

                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback?foo=bar&baz=quux"
            });

            tokenResult.IsError.Should().BeFalse();
            tokenResult.HttpErrorReason.Should().Be("OK");
            tokenResult.TokenType.Should().Be("Bearer");
            tokenResult.AccessToken.Should().NotBeNull();
            tokenResult.ExpiresIn.Should().BeGreaterThan(0);
            tokenResult.IdentityToken.Should().NotBeNull();

            var token = new JwtSecurityToken(tokenResult.IdentityToken);

            var s_hash = token.Claims.FirstOrDefault(c => c.Type == "s_hash");
            s_hash.Should().NotBeNull();
        }
    }
}
