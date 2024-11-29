using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class UserInfoEndpointClientTests
    {
        private readonly UserInfoEndpointClient _sut;

        public UserInfoEndpointClientTests()
        {
            _sut = new UserInfoEndpointClient();
        }

        [Fact]
        public async Task GetUserInfo_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Act
            var userInfo = await _sut._client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = "https://server/connect/userinfo",
                Token = "invalid_token"
            });

            // Assert
            userInfo.IsError.Should().BeTrue();
            userInfo.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUserInfo_WithValidTokenNoIdentityScope_ShouldReturnForbidden()
        {
            // Arrange
            var tokenResponse = await _sut._client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = "https://server/connect/token",
                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "api1",
                UserName = "bob",
                Password = "bob"
            });

            // Act
            var userInfo = await _sut._client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = "https://server/connect/userinfo",
                Token = tokenResponse.AccessToken
            });

            // Assert
            userInfo.IsError.Should().BeTrue();
            userInfo.HttpStatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetUserInfo_WithValidTokenAndOpenIdScope_ShouldReturnUserClaims()
        {
            // Arrange
            var tokenResponse = await _sut._client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = "https://server/connect/token",
                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "openid email api1",
                UserName = "bob",
                Password = "bob"
            });

            // Act
            var userInfo = await _sut._client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = "https://server/connect/userinfo",
                Token = tokenResponse.AccessToken
            });

            // Assert
            userInfo.IsError.Should().BeFalse();
            userInfo.Claims.Should().Contain(c => c.Type == "sub" && c.Value == "88421113");
            userInfo.Claims.Should().Contain(c => c.Type == "email" && c.Value == "BobSmith@email.com");
            userInfo.Claims.Should().Contain(c => c.Type == "email_verified" && c.Value == "true");
        }
    }
}
