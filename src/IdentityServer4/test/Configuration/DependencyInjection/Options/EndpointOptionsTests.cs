using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class EndpointOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var options = new EndpointsOptions();

            // Assert
            Assert.True(options.EnableAuthorizeEndpoint);
            Assert.False(options.EnableJwtRequestUri);
            Assert.True(options.EnableTokenEndpoint);
            Assert.True(options.EnableUserInfoEndpoint);
            Assert.True(options.EnableDiscoveryEndpoint);
            Assert.True(options.EnableEndSessionEndpoint);
            Assert.True(options.EnableCheckSessionEndpoint);
            Assert.True(options.EnableTokenRevocationEndpoint);
            Assert.True(options.EnableIntrospectionEndpoint);
            Assert.True(options.EnableDeviceAuthorizationEndpoint);
        }

        [Fact]
        public void WhenModifyingValues_ShouldPersist()
        {
            // Arrange
            var options = new EndpointsOptions
            {
                EnableAuthorizeEndpoint = false,
                EnableJwtRequestUri = true,
                EnableTokenEndpoint = false,
                EnableUserInfoEndpoint = false,
                EnableDiscoveryEndpoint = false,
                EnableEndSessionEndpoint = false,
                EnableCheckSessionEndpoint = false,
                EnableTokenRevocationEndpoint = false,
                EnableIntrospectionEndpoint = false,
                EnableDeviceAuthorizationEndpoint = false
            };

            // Assert
            Assert.False(options.EnableAuthorizeEndpoint);
            Assert.True(options.EnableJwtRequestUri);
            Assert.False(options.EnableTokenEndpoint);
            Assert.False(options.EnableUserInfoEndpoint);
            Assert.False(options.EnableDiscoveryEndpoint);
            Assert.False(options.EnableEndSessionEndpoint);
            Assert.False(options.EnableCheckSessionEndpoint);
            Assert.False(options.EnableTokenRevocationEndpoint);
            Assert.False(options.EnableIntrospectionEndpoint);
            Assert.False(options.EnableDeviceAuthorizationEndpoint);
        }
    }
}
