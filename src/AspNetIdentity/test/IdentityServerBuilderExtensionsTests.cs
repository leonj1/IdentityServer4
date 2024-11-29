using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AspNetIdentity;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityModel;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class IdentityServerBuilderExtensionsTests
    {
        private class TestUser { }
        
        [Fact]
        public void AddAspNetIdentity_ShouldConfigureRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new IdentityServerBuilder(services);

            // Act
            builder.AddAspNetIdentity<TestUser>();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify identity options configuration
            var identityOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Value;
            identityOptions.ClaimsIdentity.UserIdClaimType.Should().Be(JwtClaimTypes.Subject);
            identityOptions.ClaimsIdentity.UserNameClaimType.Should().Be(JwtClaimTypes.Name);
            identityOptions.ClaimsIdentity.RoleClaimType.Should().Be(JwtClaimTypes.Role);

            // Verify cookie configuration
            var cookieOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CookieAuthenticationOptions>>().Value;
            cookieOptions.Cookie.IsEssential.Should().BeTrue();
            cookieOptions.Cookie.SameSite.Should().Be(Microsoft.AspNetCore.Http.SameSiteMode.None);

            // Verify required services are registered
            services.Should().Contain(x => x.ServiceType == typeof(IUserClaimsPrincipalFactory<TestUser>));
            services.Should().Contain(x => x.ServiceType == typeof(IResourceOwnerPasswordValidator));
            services.Should().Contain(x => x.ServiceType == typeof(IProfileService));
        }

        [Fact]
        public void AddTransientDecorator_ShouldRegisterDecorator()
        {
            // Arrange
            var services = new ServiceCollection();
            
            // Act & Assert
            services.AddTransient<IUserClaimsPrincipalFactory<TestUser>, UserClaimsFactory<TestUser>>();
            
            Action act = () => services.AddTransientDecorator<IUserClaimsPrincipalFactory<TestUser>, UserClaimsFactory<TestUser>>();
            
            act.Should().NotThrow();
            services.Should().Contain(x => x.ServiceType == typeof(Decorator<IUserClaimsPrincipalFactory<TestUser>>));
        }

        [Fact]
        public void AddDecorator_WithoutServiceRegistered_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Action act = () => services.AddDecorator<IUserClaimsPrincipalFactory<TestUser>>();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Service type: IUserClaimsPrincipalFactory`1 not registered.");
        }

        [Fact]
        public void AddDecorator_WhenAlreadyRegistered_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTransient<IUserClaimsPrincipalFactory<TestUser>, UserClaimsFactory<TestUser>>();
            services.AddDecorator<IUserClaimsPrincipalFactory<TestUser>>();

            // Act & Assert
            Action act = () => services.AddDecorator<IUserClaimsPrincipalFactory<TestUser>>();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Decorator already registered for type: IUserClaimsPrincipalFactory`1.");
        }
    }
}
