using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class ApiAuthenticationFailureEventTests
    {
        [Fact]
        public void Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var apiName = "test_api";
            var message = "Authentication failed";

            // Act
            var evt = new ApiAuthenticationFailureEvent(apiName, message);

            // Assert
            Assert.Equal(EventCategories.Authentication, evt.Category);
            Assert.Equal("API Authentication Failure", evt.Name);
            Assert.Equal(EventTypes.Failure, evt.EventType);
            Assert.Equal(EventIds.ApiAuthenticationFailure, evt.Id);
            Assert.Equal(message, evt.Message);
            Assert.Equal(apiName, evt.ApiName);
        }
    }
}
