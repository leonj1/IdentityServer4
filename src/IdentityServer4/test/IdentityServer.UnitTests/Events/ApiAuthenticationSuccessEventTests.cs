using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class ApiAuthenticationSuccessEventTests
    {
        [Fact]
        public void Constructor_ShouldSetCorrectProperties()
        {
            // Arrange
            var apiName = "test_api";
            var authMethod = "bearer_token";

            // Act
            var evt = new ApiAuthenticationSuccessEvent(apiName, authMethod);

            // Assert
            Assert.Equal(EventCategories.Authentication, evt.Category);
            Assert.Equal("API Authentication Success", evt.Name);
            Assert.Equal(EventTypes.Success, evt.Type);
            Assert.Equal(EventIds.ApiAuthenticationSuccess, evt.Id);
            Assert.Equal(apiName, evt.ApiName);
            Assert.Equal(authMethod, evt.AuthenticationMethod);
        }

        [Theory]
        [InlineData("api1", "bearer")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Constructor_WithDifferentInputs_ShouldSetProperties(string apiName, string authMethod)
        {
            // Act
            var evt = new ApiAuthenticationSuccessEvent(apiName, authMethod);

            // Assert
            Assert.Equal(apiName, evt.ApiName);
            Assert.Equal(authMethod, evt.AuthenticationMethod);
        }
    }
}
