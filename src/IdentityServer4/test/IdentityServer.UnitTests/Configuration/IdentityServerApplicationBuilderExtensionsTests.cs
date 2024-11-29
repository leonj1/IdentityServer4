using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class IdentityServerApplicationBuilderExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly IApplicationBuilder _app;

        public IdentityServerApplicationBuilderExtensionsTests()
        {
            _services = new ServiceCollection();
            _services.AddLogging();
            
            // Add required services
            _services.AddSingleton<IAuthenticationSchemeProvider, MockAuthenticationSchemeProvider>();
            _services.AddSingleton<IdentityServerOptions>();
            
            var serviceProvider = _services.BuildServiceProvider();
            _app = new ApplicationBuilder(serviceProvider);
        }

        [Fact]
        public void UseIdentityServer_WithNullApp_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                IdentityServerApplicationBuilderExtensions.UseIdentityServer(null));
        }

        [Fact]
        public void Validate_WithMissingRequiredServices_ThrowsInvalidOperationException()
        {
            // Arrange
            var serviceProvider = _services.BuildServiceProvider();
            var app = new ApplicationBuilder(serviceProvider);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                app.Validate());
        }

        [Fact]
        public void TestService_WithMissingRequiredService_ThrowsInvalidOperationException()
        {
            // Arrange
            var serviceProvider = _services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<IdentityServerApplicationBuilderExtensionsTests>>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                IdentityServerApplicationBuilderExtensions.TestService(
                    serviceProvider, 
                    typeof(IPersistedGrantStore), 
                    logger));
        }

        [Fact]
        public void TestService_WithRequiredService_ReturnsService()
        {
            // Arrange
            _services.AddSingleton<IPersistedGrantStore, InMemoryPersistedGrantStore>();
            var serviceProvider = _services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<IdentityServerApplicationBuilderExtensionsTests>>();

            // Act
            var result = IdentityServerApplicationBuilderExtensions.TestService(
                serviceProvider,
                typeof(IPersistedGrantStore),
                logger);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IPersistedGrantStore>();
        }
    }
}
