using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.IntegrationTests.Common
{
    public class IdentityServerPipelineTests
    {
        private IdentityServerPipeline _pipeline;

        public IdentityServerPipelineTests()
        {
            _pipeline = new IdentityServerPipeline();
            _pipeline.Initialize();
        }

        [Fact]
        public async Task Login_Should_Set_LoginWasCalled()
        {
            // Arrange
            var subject = new IdentityServerUser("123").CreatePrincipal();

            // Act
            await _pipeline.LoginAsync(subject);

            // Assert
            _pipeline.LoginWasCalled.Should().BeTrue();
        }

        [Fact]
        public void CreateAuthorizeUrl_Should_Return_Valid_Url()
        {
            // Arrange
            var clientId = "client";
            var responseType = "code";
            var scope = "openid profile";
            var redirectUri = "https://client/callback";

            // Act
            var url = _pipeline.CreateAuthorizeUrl(
                clientId: clientId,
                responseType: responseType,
                scope: scope,
                redirectUri: redirectUri);

            // Assert
            url.Should().Contain($"client_id={clientId}");
            url.Should().Contain($"response_type={responseType}");
            url.Should().Contain($"scope={scope.Replace(" ", "+")}");
            url.Should().Contain($"redirect_uri={Uri.EscapeDataString(redirectUri)}");
        }

        [Fact]
        public void RemoveLoginCookie_Should_Remove_Cookie()
        {
            // Act
            _pipeline.RemoveLoginCookie();

            // Assert - verify cookie is not present
            _pipeline.BrowserClient.GetCookie(_pipeline.BaseUrl, 
                IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme)
                .Should().BeNull();
        }

        [Fact]
        public async Task Logout_Should_Set_LogoutWasCalled()
        {
            // Arrange
            var subject = new IdentityServerUser("123").CreatePrincipal();
            await _pipeline.LoginAsync(subject);

            // Act
            await _pipeline.BrowserClient.GetAsync(_pipeline.BaseUrl + "/account/logout");

            // Assert
            _pipeline.LogoutWasCalled.Should().BeTrue();
        }

        [Fact]
        public void Initialize_Should_Setup_Default_Endpoints()
        {
            // Arrange
            var newPipeline = new IdentityServerPipeline();

            // Act
            newPipeline.Initialize();

            // Assert
            newPipeline.DiscoveryEndpoint.Should().NotBeNullOrEmpty();
            newPipeline.AuthorizeEndpoint.Should().NotBeNullOrEmpty();
            newPipeline.TokenEndpoint.Should().NotBeNullOrEmpty();
        }
    }
}
