using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class ClientAuthenticationTestsAdditional : ClientAuthenticationTests
    {
        private const string Category = "Conformance.Basic.ClientAuthenticationTests.Additional";

        [Fact]
        [Trait("Category", Category)]
        public async Task Token_endpoint_should_fail_with_invalid_client_secret()
        {
            await _pipeline.LoginAsync("bob");

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
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client with wrong secret
            var wrapper = new MessageHandlerWrapper(_pipeline.Handler);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "wrong_secret",
                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback"
            });

            tokenResult.IsError.Should().BeTrue();
            tokenResult.Error.Should().Be("invalid_client");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Token_endpoint_should_fail_with_missing_client_credentials()
        {
            await _pipeline.LoginAsync("bob");

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
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client with no credentials
            var wrapper = new MessageHandlerWrapper(_pipeline.Handler);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback"
            });

            tokenResult.IsError.Should().BeTrue();
            tokenResult.Error.Should().Be("invalid_client");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Token_endpoint_should_fail_with_invalid_redirect_uri()
        {
            await _pipeline.LoginAsync("bob");

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
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client with wrong redirect URI
            var wrapper = new MessageHandlerWrapper(_pipeline.Handler);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",
                Code = code,
                RedirectUri = "https://wrong.url/callback"
            });

            tokenResult.IsError.Should().BeTrue();
            tokenResult.Error.Should().Be("invalid_grant");
        }
    }
}
