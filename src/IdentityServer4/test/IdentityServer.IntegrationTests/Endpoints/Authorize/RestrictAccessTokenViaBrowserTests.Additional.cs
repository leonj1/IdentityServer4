using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Authorize
{
    public class RestrictAccessTokenViaBrowserAdditionalTests
    {
        private const string Category = "RestrictAccessTokenViaBrowserAdditionalTests";
        private IdentityServerPipeline _mockPipeline;
        private ClaimsPrincipal _user;

        public RestrictAccessTokenViaBrowserAdditionalTests()
        {
            _mockPipeline = new IdentityServerPipeline();
            _user = new IdentityServerUser("bob").CreatePrincipal();
            
            // Setup is similar to main test class but with specific test cases
            _mockPipeline.Initialize();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unauthenticated_User_Should_Be_Redirected_To_Login()
        {
            // Don't login the user
            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "state",
                nonce: "nonce");

            _mockPipeline.BrowserClient.AllowAutoRedirect = false;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Found);
            response.Headers.Location.ToString().Should().Contain("/connect/authorize/login");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_RedirectUri_Should_Show_Error()
        {
            await _mockPipeline.LoginAsync(_user);

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://invalid/callback",
                state: "state",
                nonce: "nonce");

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);
            
            _mockPipeline.ErrorWasCalled.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Nonce_Should_Show_Error()
        {
            await _mockPipeline.LoginAsync(_user);

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "state",
                nonce: null);

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);
            
            _mockPipeline.ErrorWasCalled.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Scope_Should_Show_Error()
        {
            await _mockPipeline.LoginAsync(_user);

            var url = _mockPipeline.CreateAuthorizeUrl(
                clientId: "client1",
                responseType: "id_token",
                scope: "invalid_scope",
                redirectUri: "https://client1/callback",
                state: "state",
                nonce: "nonce");

            _mockPipeline.BrowserClient.AllowAutoRedirect = true;
            var response = await _mockPipeline.BrowserClient.GetAsync(url);
            
            _mockPipeline.ErrorWasCalled.Should().BeTrue();
        }
    }
}
