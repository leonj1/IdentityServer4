using System;
using FluentAssertions;
using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class CachingOptionsTests
    {
        [Fact]
        public void DefaultValues_ShouldBe_15Minutes()
        {
            // Arrange
            var options = new CachingOptions();
            var expected = TimeSpan.FromMinutes(15);

            // Assert
            options.ClientStoreExpiration.Should().Be(expected);
            options.ResourceStoreExpiration.Should().Be(expected);
            options.CorsExpiration.Should().Be(expected);
        }

        [Fact]
        public void SettingCustomValues_ShouldWork()
        {
            // Arrange
            var options = new CachingOptions
            {
                ClientStoreExpiration = TimeSpan.FromMinutes(30),
                ResourceStoreExpiration = TimeSpan.FromHours(1),
                CorsExpiration = TimeSpan.FromMinutes(45)
            };

            // Assert
            options.ClientStoreExpiration.Should().Be(TimeSpan.FromMinutes(30));
            options.ResourceStoreExpiration.Should().Be(TimeSpan.FromHours(1));
            options.CorsExpiration.Should().Be(TimeSpan.FromMinutes(45));
        }
    }
}
