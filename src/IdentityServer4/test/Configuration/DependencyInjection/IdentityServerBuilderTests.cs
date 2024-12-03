using System;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class IdentityServerBuilderTests
    {
        [Fact]
        public void Constructor_WhenServicesIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new IdentityServerBuilder(null));
        }

        [Fact]
        public void Constructor_WhenServicesIsValid_SetsServicesProperty()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var builder = new IdentityServerBuilder(services);

            // Assert
            Assert.Same(services, builder.Services);
        }

        [Fact]
        public void Services_Property_ReturnsExpectedServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new IdentityServerBuilder(services);

            // Act & Assert
            Assert.NotNull(builder.Services);
            Assert.IsType<ServiceCollection>(builder.Services);
        }
    }
}
