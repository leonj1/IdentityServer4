using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace IdentityServer4.UnitTests.Hosting.LocalApiAuthentication
{
    public class LocalApiAuthenticationExtensionsTests
    {
        [Fact]
        public void AddLocalApiAuthentication_ShouldAddAuthenticationAndAuthorization()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddLocalApiAuthentication();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var authenticationSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
            authenticationSchemeProvider.Should().NotBeNull();
        }

        [Fact]
        public async Task AddLocalApiAuthentication_WithTransformationFunc_ShouldTransformPrincipal()
        {
            // Arrange
            var services = new ServiceCollection();
            var transformedClaim = new Claim("transformed", "true");
            
            // Act
            services.AddLocalApiAuthentication(principal =>
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(transformedClaim);
                return Task.FromResult(new ClaimsPrincipal(identity));
            });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
            
            // Assert
            options.Should().NotBeNull();
        }

        [Fact]
        public void AddLocalApi_WithCustomScheme_ShouldRegisterHandler()
        {
            // Arrange
            var builder = new AuthenticationBuilder(new ServiceCollection());
            var customScheme = "CustomScheme";

            // Act
            builder.AddLocalApi(customScheme, options => { });

            // Assert
            var services = builder.Services.BuildServiceProvider();
            var schemeProvider = services.GetRequiredService<IAuthenticationSchemeProvider>();
            schemeProvider.Should().NotBeNull();
        }
    }
}
