using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class RedirectUriTestsAdditional : RedirectUriTests
    {
        private const string Category = "Conformance.Basic.RedirectUriTestsAdditional";

        [Fact]
        [Trait("Category", Category)]
        public async Task Accept_Valid_Redirect_Uri_With_No_Query_Parameters()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            _mockPipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_client/callback",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().StartWith("https://code_client/callback?");
            var authorization = _mockPipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();
            authorization.State.Should().Be(state);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Reject_Redirect_Uri_With_Fragment()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_client/callback#fragment",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorWasCalled.Should().BeTrue();
            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Reject_Redirect_Uri_With_Different_Case()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://CODE_CLIENT/callback",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorWasCalled.Should().BeTrue();
            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }
    }
}
