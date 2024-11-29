using System;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class CorsOptionsTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var options = new CorsOptions();

            // Assert
            options.CorsPolicyName.Should().Be(Constants.IdentityServerName);
            options.PreflightCacheDuration.Should().BeNull();
            options.CorsPaths.Should().NotBeNull();
            options.CorsPaths.Should().NotBeEmpty();
            options.CorsPaths.Should().AllBeOfType<PathString>();
        }

        [Fact]
        public void CorsPaths_ShouldHaveLeadingSlash()
        {
            // Act
            var options = new CorsOptions();

            // Assert
            options.CorsPaths.Should().OnlyContain(path => path.Value.StartsWith("/"));
        }

        [Fact]
        public void PreflightCacheDuration_CanBeSet()
        {
            // Arrange
            var options = new CorsOptions();
            var duration = TimeSpan.FromMinutes(10);

            // Act
            options.PreflightCacheDuration = duration;

            // Assert
            options.PreflightCacheDuration.Should().Be(duration);
        }

        [Fact]
        public void CorsPolicyName_CanBeModified()
        {
            // Arrange
            var options = new CorsOptions();
            var newPolicyName = "CustomPolicy";

            // Act
            options.CorsPolicyName = newPolicyName;

            // Assert
            options.CorsPolicyName.Should().Be(newPolicyName);
        }
    }
}
