using System;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Xunit;
using static IdentityServer4.Constants;

namespace IdentityServer4.UnitTests.Extensions
{
    public class EndpointOptionsExtensionsTests
    {
        private readonly EndpointsOptions _options;

        public EndpointOptionsExtensionsTests()
        {
            _options = new EndpointsOptions();
        }

        [Fact]
        public void IsEndpointEnabled_WhenEndpointIsNull_ShouldReturnTrue()
        {
            _options.IsEndpointEnabled(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(EndpointNames.Authorize)]
        [InlineData(EndpointNames.CheckSession)]
        [InlineData(EndpointNames.DeviceAuthorization)]
        [InlineData(EndpointNames.Discovery)]
        [InlineData(EndpointNames.EndSession)]
        [InlineData(EndpointNames.Introspection)]
        [InlineData(EndpointNames.Revocation)]
        [InlineData(EndpointNames.Token)]
        [InlineData(EndpointNames.UserInfo)]
        public void IsEndpointEnabled_WhenEndpointIsEnabled_ShouldReturnTrue(string endpointName)
        {
            // Arrange
            var endpoint = new Endpoint(endpointName, "", null);
            EnableAllEndpoints();

            // Act
            var result = _options.IsEndpointEnabled(endpoint);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(EndpointNames.Authorize, nameof(EndpointsOptions.EnableAuthorizeEndpoint))]
        [InlineData(EndpointNames.CheckSession, nameof(EndpointsOptions.EnableCheckSessionEndpoint))]
        [InlineData(EndpointNames.DeviceAuthorization, nameof(EndpointsOptions.EnableDeviceAuthorizationEndpoint))]
        [InlineData(EndpointNames.Discovery, nameof(EndpointsOptions.EnableDiscoveryEndpoint))]
        [InlineData(EndpointNames.EndSession, nameof(EndpointsOptions.EnableEndSessionEndpoint))]
        [InlineData(EndpointNames.Introspection, nameof(EndpointsOptions.EnableIntrospectionEndpoint))]
        [InlineData(EndpointNames.Revocation, nameof(EndpointsOptions.EnableTokenRevocationEndpoint))]
        [InlineData(EndpointNames.Token, nameof(EndpointsOptions.EnableTokenEndpoint))]
        [InlineData(EndpointNames.UserInfo, nameof(EndpointsOptions.EnableUserInfoEndpoint))]
        public void IsEndpointEnabled_WhenEndpointIsDisabled_ShouldReturnFalse(string endpointName, string propertyName)
        {
            // Arrange
            var endpoint = new Endpoint(endpointName, "", null);
            EnableAllEndpoints();
            typeof(EndpointsOptions).GetProperty(propertyName).SetValue(_options, false);

            // Act
            var result = _options.IsEndpointEnabled(endpoint);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsEndpointEnabled_WhenEndpointNameIsUnknown_ShouldReturnTrue()
        {
            // Arrange
            var endpoint = new Endpoint("Unknown", "", null);

            // Act
            var result = _options.IsEndpointEnabled(endpoint);

            // Assert
            result.Should().BeTrue();
        }

        private void EnableAllEndpoints()
        {
            _options.EnableAuthorizeEndpoint = true;
            _options.EnableCheckSessionEndpoint = true;
            _options.EnableDeviceAuthorizationEndpoint = true;
            _options.EnableDiscoveryEndpoint = true;
            _options.EnableEndSessionEndpoint = true;
            _options.EnableIntrospectionEndpoint = true;
            _options.EnableTokenRevocationEndpoint = true;
            _options.EnableTokenEndpoint = true;
            _options.EnableUserInfoEndpoint = true;
        }
    }
}
