using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xunit;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer4.UnitTests.Models
{
    public class LogoutRequestTests
    {
        [Fact]
        public void LogoutMessage_Constructor_WithNullRequest_ShouldCreateEmptyMessage()
        {
            // Arrange & Act
            var message = new LogoutMessage(null);

            // Assert
            Assert.Empty(message.Parameters);
            Assert.Null(message.ClientId);
            Assert.Null(message.ClientName);
            Assert.Null(message.SubjectId);
            Assert.Null(message.SessionId);
            Assert.Null(message.ClientIds);
            Assert.Null(message.PostLogoutRedirectUri);
        }

        [Fact]
        public void LogoutRequest_Constructor_WithValidMessage_ShouldSetProperties()
        {
            // Arrange
            var message = new LogoutMessage
            {
                ClientId = "client1",
                ClientName = "Test Client",
                PostLogoutRedirectUri = "https://client1/signout-callback",
                SubjectId = "123",
                SessionId = "session1",
                ClientIds = new[] { "client1", "client2" }
            };

            // Act
            var request = new LogoutRequest("https://test/signout", message);

            // Assert
            Assert.Equal("client1", request.ClientId);
            Assert.Equal("Test Client", request.ClientName);
            Assert.Equal("https://client1/signout-callback", request.PostLogoutRedirectUri);
            Assert.Equal("123", request.SubjectId);
            Assert.Equal("session1", request.SessionId);
            Assert.Equal(2, request.ClientIds.Count());
            Assert.Contains("client1", request.ClientIds);
            Assert.Contains("client2", request.ClientIds);
            Assert.Equal("https://test/signout", request.SignOutIFrameUrl);
        }

        [Fact]
        public void ShowSignoutPrompt_WhenClientIdMissing_ShouldReturnTrue()
        {
            // Arrange
            var request = new LogoutRequest("https://test/signout", new LogoutMessage());

            // Assert
            Assert.True(request.ShowSignoutPrompt);
        }

        [Fact]
        public void ShowSignoutPrompt_WhenClientIdPresent_ShouldReturnFalse()
        {
            // Arrange
            var message = new LogoutMessage { ClientId = "client1" };
            var request = new LogoutRequest("https://test/signout", message);

            // Assert
            Assert.False(request.ShowSignoutPrompt);
        }

        [Fact]
        public void LogoutMessage_ContainsPayload_WhenClientIdPresent_ShouldReturnTrue()
        {
            // Arrange
            var message = new LogoutMessage { ClientId = "client1" };

            // Assert
            Assert.True(message.ContainsPayload);
        }

        [Fact]
        public void LogoutMessage_ContainsPayload_WhenClientIdsPresent_ShouldReturnTrue()
        {
            // Arrange
            var message = new LogoutMessage { ClientIds = new[] { "client1" } };

            // Assert
            Assert.True(message.ContainsPayload);
        }

        [Fact]
        public void LogoutMessage_ContainsPayload_WhenNoData_ShouldReturnFalse()
        {
            // Arrange
            var message = new LogoutMessage();

            // Assert
            Assert.False(message.ContainsPayload);
        }
    }
}
