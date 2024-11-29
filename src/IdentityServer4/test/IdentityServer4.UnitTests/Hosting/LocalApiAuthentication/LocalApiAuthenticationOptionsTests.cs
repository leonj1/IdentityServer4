using IdentityServer4.Hosting.LocalApiAuthentication;
using FluentAssertions;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting.LocalApiAuthentication
{
    public class LocalApiAuthenticationOptionsTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Arrange & Act
            var options = new LocalApiAuthenticationOptions();

            // Assert
            options.SaveToken.Should().BeTrue();
            options.ExpectedScope.Should().BeNull();
            options.Events.Should().BeNull();
        }

        [Fact]
        public void SettingProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var options = new LocalApiAuthenticationOptions();
            var events = new LocalApiAuthenticationEvents();

            // Act
            options.ExpectedScope = "api1";
            options.SaveToken = false;
            options.Events = events;

            // Assert
            options.ExpectedScope.Should().Be("api1");
            options.SaveToken.Should().BeFalse();
            options.Events.Should().BeSameAs(events);
        }
    }
}
