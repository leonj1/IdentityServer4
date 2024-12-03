using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Models.Messages
{
    public class LogoutRequestTests
    {
        [Fact]
        public void LogoutMessage_Constructor_WithNullRequest_ShouldCreateEmptyInstance()
        {
            var message = new LogoutMessage(null);

            message.Parameters.Should().BeEmpty();
            message.ClientId.Should().BeNull();
            message.ClientName.Should().BeNull();
            message.SubjectId.Should().BeNull();
            message.SessionId.Should().BeNull();
            message.ClientIds.Should().BeNull();
            message.PostLogoutRedirectUri.Should().BeNull();
        }

        [Fact]
        public void LogoutMessage_Constructor_WithValidRequest_ShouldSetProperties()
        {
            var raw = new NameValueCollection
            {
                { "param1", "value1" },
                { OidcConstants.EndSessionRequest.IdTokenHint, "token" },
                { OidcConstants.EndSessionRequest.PostLogoutRedirectUri, "uri" },
                { OidcConstants.EndSessionRequest.State, "state" }
            };

            var request = new ValidatedEndSessionRequest
            {
                Raw = raw,
                Client = new Client { ClientId = "client1", ClientName = "Test Client" },
                SessionId = "session1",
                Subject = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(
                        new[] { new System.Security.Claims.Claim("sub", "123") })),
                ClientIds = new[] { "client1", "client2" },
                PostLogOutUri = "http://localhost/logout"
            };

            var message = new LogoutMessage(request);

            message.ClientId.Should().Be("client1");
            message.ClientName.Should().Be("Test Client");
            message.SubjectId.Should().Be("123");
            message.SessionId.Should().Be("session1");
            message.ClientIds.Should().BeEquivalentTo(new[] { "client1", "client2" });
            message.PostLogoutRedirectUri.Should().StartWith("http://localhost/logout");
            message.ContainsPayload.Should().BeTrue();
        }

        [Fact]
        public void LogoutRequest_Constructor_ShouldInitializeFromMessage()
        {
            var message = new LogoutMessage
            {
                ClientId = "client1",
                ClientName = "Test Client",
                SubjectId = "123",
                SessionId = "session1",
                ClientIds = new[] { "client1", "client2" },
                PostLogoutRedirectUri = "http://localhost/logout",
                Parameters = new Dictionary<string, string[]> { { "param1", new[] { "value1" } } }
            };

            var request = new LogoutRequest("http://iframe-url", message);

            request.ClientId.Should().Be("client1");
            request.ClientName.Should().Be("Test Client");
            request.SubjectId.Should().Be("123");
            request.SessionId.Should().Be("session1");
            request.ClientIds.Should().BeEquivalentTo(new[] { "client1", "client2" });
            request.PostLogoutRedirectUri.Should().Be("http://localhost/logout");
            request.SignOutIFrameUrl.Should().Be("http://iframe-url");
            request.ShowSignoutPrompt.Should().BeFalse();
        }

        [Fact]
        public void LogoutRequest_ShowSignoutPrompt_ShouldBeTrueWhenClientIdMissing()
        {
            var request = new LogoutRequest("http://iframe-url", new LogoutMessage());
            request.ShowSignoutPrompt.Should().BeTrue();
        }
    }
}
