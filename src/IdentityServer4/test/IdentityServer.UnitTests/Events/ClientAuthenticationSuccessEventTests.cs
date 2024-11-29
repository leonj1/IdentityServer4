using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class ClientAuthenticationSuccessEventTests
    {
        [Fact]
        public void Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var clientId = "test_client";
            var authMethod = "client_secret_basic";

            // Act
            var evt = new ClientAuthenticationSuccessEvent(clientId, authMethod);

            // Assert
            Assert.Equal(EventCategories.Authentication, evt.Category);
            Assert.Equal("Client Authentication Success", evt.Name);
            Assert.Equal(EventTypes.Success, evt.Type);
            Assert.Equal(EventIds.ClientAuthenticationSuccess, evt.Id);
            Assert.Equal(clientId, evt.ClientId);
            Assert.Equal(authMethod, evt.AuthenticationMethod);
        }

        [Fact]
        public void Properties_ShouldAllowModification()
        {
            // Arrange
            var evt = new ClientAuthenticationSuccessEvent("initial_client", "initial_method");
            
            // Act
            evt.ClientId = "modified_client";
            evt.AuthenticationMethod = "modified_method";

            // Assert
            Assert.Equal("modified_client", evt.ClientId);
            Assert.Equal("modified_method", evt.AuthenticationMethod);
        }
    }
}
