using System;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class LogoutMessageTests
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
    }
}
