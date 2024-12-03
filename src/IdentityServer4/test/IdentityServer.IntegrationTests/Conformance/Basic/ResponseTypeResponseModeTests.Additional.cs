using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class ResponseTypeResponseModeTestsAdditional : ResponseTypeResponseModeTests
    {
        private const string Category = "Conformance.Basic.ResponseTypeResponseModeTests";

        [Fact]
        [Trait("Category", Category)]
        public async Task Request_with_invalid_response_type_should_fail()
        {
            await _mockPipeline.LoginAsync("bob");

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "code_client",
                responseType: "invalid_type",
                scope: "openid",
                redirectUri: "https://code_client/callback",
                state: state,
                nonce: nonce);

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorMessage.Error.Should().Be("unsupported_response_type");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Request_with_invalid_redirect_uri_should_fail()
        {
            await _mockPipeline.LoginAsync("bob");

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "code_client",
                responseType: "code",
                scope: "openid",
                redirectUri: "https://invalid_client/callback",
                state: state,
                nonce: nonce);

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Request_without_scope_should_fail()
        {
            await _mockPipeline.LoginAsync("bob");

            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "code_client",
                responseType: "code",
                scope: "",
                redirectUri: "https://code_client/callback",
                state: state,
                nonce: nonce);

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }
    }
}
