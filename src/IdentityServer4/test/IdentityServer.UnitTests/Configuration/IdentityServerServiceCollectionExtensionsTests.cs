using FluentAssertions;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class IdentityServerServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection _services;

        public IdentityServerServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddIdentityServer_ShouldReturnBuilder()
        {
            // Act
            var builder = _services.AddIdentityServer();

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<IdentityServerBuilder>();
        }

        [Fact]
        public void AddIdentityServer_WithOptions_ShouldConfigureOptions()
        {
            // Arrange
            var optionsConfigured = false;

            // Act
            _services.AddIdentityServer(options => 
            {
                optionsConfigured = true;
            });

            // Assert
            optionsConfigured.Should().BeTrue();
        }

        [Fact]
        public void AddIdentityServer_WithConfiguration_ShouldConfigureOptions()
        {
            // Arrange
            var configurationValues = new Dictionary<string, string>
            {
                {"Issuer", "https://test.com"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            // Act
            var builder = _services.AddIdentityServer(configuration);

            // Assert
            builder.Should().NotBeNull();
            var sp = _services.BuildServiceProvider();
            var options = sp.GetRequiredService<IOptions<IdentityServerOptions>>().Value;
            options.IssuerUri.Should().Be("https://test.com");
        }

        [Fact]
        public void AddOidcStateDataFormatterCache_ShouldRegisterPostConfigureOptions()
        {
            // Act
            _services.AddOidcStateDataFormatterCache("test-scheme");

            // Assert
            var descriptor = _services.FirstOrDefault(x => 
                x.ServiceType == typeof(IPostConfigureOptions<OpenIdConnectOptions>));
            descriptor.Should().NotBeNull();
            descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }
    }
}
