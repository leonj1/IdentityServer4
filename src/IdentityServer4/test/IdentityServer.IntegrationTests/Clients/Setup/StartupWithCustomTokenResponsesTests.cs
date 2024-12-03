using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Xunit;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class StartupWithCustomTokenResponsesTests
    {
        [Fact]
        public void ConfigureServices_ShouldRegisterRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var startup = new StartupWithCustomTokenResponses();

            // Act
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<IIdentityServerBuilder>());
            Assert.NotNull(serviceProvider.GetService<IResourceOwnerPasswordValidator>());
            Assert.NotNull(serviceProvider.GetService<IExtensionGrantValidator>());
        }

        [Fact]
        public void Configure_ShouldAddIdentityServerMiddleware()
        {
            // Arrange
            var startup = new StartupWithCustomTokenResponses();
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            
            var appBuilder = new ApplicationBuilder(serviceProvider);

            // Act
            startup.Configure(appBuilder);

            // Assert
            var middleware = appBuilder.Build();
            Assert.NotNull(middleware);
        }

        [Fact]
        public void ConfigureServices_ShouldConfigureCorrectIssuerUri()
        {
            // Arrange
            var services = new ServiceCollection();
            var startup = new StartupWithCustomTokenResponses();

            // Act
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IdentityServerOptions>();

            // Assert
            Assert.Equal("https://idsvr4", options.IssuerUri);
        }

        [Fact]
        public void ConfigureServices_ShouldEnableAllEvents()
        {
            // Arrange
            var services = new ServiceCollection();
            var startup = new StartupWithCustomTokenResponses();

            // Act
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IdentityServerOptions>();

            // Assert
            Assert.True(options.Events.RaiseErrorEvents);
            Assert.True(options.Events.RaiseFailureEvents);
            Assert.True(options.Events.RaiseInformationEvents);
            Assert.True(options.Events.RaiseSuccessEvents);
        }
    }
}
