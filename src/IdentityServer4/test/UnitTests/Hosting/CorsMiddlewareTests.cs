using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class CorsMiddlewareTests
    {
        [Fact]
        public void ConfigureCors_ShouldUseCorsWithCorrectPolicyName()
        {
            // Arrange
            var services = new ServiceCollection();
            var options = new IdentityServerOptions();
            options.Cors.CorsPolicyName = "TestPolicy";
            
            services.AddSingleton(options);
            var serviceProvider = services.BuildServiceProvider();
            
            var appBuilder = new ApplicationBuilder(serviceProvider);
            var corsServiceWasCalled = false;
            var policyNameUsed = string.Empty;
            
            appBuilder.UseCors = (policyName) => {
                corsServiceWasCalled = true;
                policyNameUsed = policyName;
                return appBuilder;
            };

            // Act
            appBuilder.ConfigureCors();

            // Assert
            corsServiceWasCalled.Should().BeTrue();
            policyNameUsed.Should().Be("TestPolicy");
        }
    }
}
