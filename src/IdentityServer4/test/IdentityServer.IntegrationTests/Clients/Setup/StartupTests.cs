using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Xunit;
using IdentityServer4.Validation;
using System.Linq;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_WithDefaultSetup_RegistersRequiredServices()
        {
            // Arrange
            var startup = new Startup();
            var services = new ServiceCollection();

            // Act
            startup.ConfigureServices(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<ITokenRequestValidator>());
            Assert.Contains(services, x => x.ServiceType == typeof(ExtensionGrantValidator));
            Assert.Contains(services, x => x.ServiceType == typeof(ExtensionGrantValidator2));
        }

        [Fact]
        public void ConfigureServices_WithCustomTokenRequestValidator_RegistersCustomValidator()
        {
            // Arrange
            var startup = new Startup();
            var services = new ServiceCollection();
            var customValidator = new CustomTokenRequestValidatorStub();
            Startup.CustomTokenRequestValidator = customValidator;

            // Act
            startup.ConfigureServices(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var resolvedValidator = serviceProvider.GetService<ICustomTokenRequestValidator>();
            Assert.Same(customValidator, resolvedValidator);
        }

        private class CustomTokenRequestValidatorStub : ICustomTokenRequestValidator
        {
            public Task ValidateAsync(CustomTokenRequestValidationContext context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
