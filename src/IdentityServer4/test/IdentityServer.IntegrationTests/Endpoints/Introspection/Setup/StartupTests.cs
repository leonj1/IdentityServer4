using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;
using IdentityServer4.Configuration;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_ShouldConfigureIdentityServer()
        {
            // Arrange
            var services = new ServiceCollection();
            var startup = new Startup();

            // Act
            startup.ConfigureServices(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IdentityServerOptions>();
            
            options.IssuerUri.Should().Be("https://idsvr4");
            options.Endpoints.EnableAuthorizeEndpoint.Should().BeFalse();
        }

        [Fact]
        public void Configure_ShouldAddIdentityServerMiddleware()
        {
            // Arrange
            var services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            
            var serviceProvider = services.BuildServiceProvider();
            var app = new ApplicationBuilder(serviceProvider);
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Act & Assert
            // This should not throw an exception
            startup.Configure(app, loggerFactory);
        }
    }
}
