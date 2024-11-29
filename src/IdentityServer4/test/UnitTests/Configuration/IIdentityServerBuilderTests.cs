using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class IIdentityServerBuilderTests
    {
        [Fact]
        public void ServicesProperty_ShouldNotBeNull()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new IdentityServerBuilder(services);

            // Act
            var result = builder.Services;

            // Assert
            Assert.NotNull(result);
            Assert.Same(services, result);
        }
    }

    // Helper implementation for testing
    internal class IdentityServerBuilder : IIdentityServerBuilder
    {
        public IdentityServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
