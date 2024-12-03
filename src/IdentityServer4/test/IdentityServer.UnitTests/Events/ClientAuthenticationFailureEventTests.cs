using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class ClientAuthenticationFailureEventTests
    {
        [Fact]
        public void Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var clientId = "test_client";
            var message = "Authentication failed";

            // Act
            var evt = new ClientAuthenticationFailureEvent(clientId, message);

            // Assert
            Assert.Equal(EventCategories.Authentication, evt.Category);
            Assert.Equal("Client Authentication Failure", evt.Name);
            Assert.Equal(EventTypes.Failure, evt.EventType);
            Assert.Equal(EventIds.ClientAuthenticationFailure, evt.Id);
            Assert.Equal(message, evt.Message);
            Assert.Equal(clientId, evt.ClientId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("client123")]
        public void ClientId_ShouldBeSettable(string clientId)
        {
            // Arrange
            var evt = new ClientAuthenticationFailureEvent("initial", "message");

            // Act
            evt.ClientId = clientId;

            // Assert
            Assert.Equal(clientId, evt.ClientId);
        }
    }
}
