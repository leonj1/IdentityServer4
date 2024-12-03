using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MockAuthenticationService = IdentityServer.UnitTests.Common.MockAuthenticationService;
using MockAuthenticationSchemeProvider = IdentityServer.UnitTests.Common.MockAuthenticationSchemeProvider;

namespace IdentityServer.UnitTests.Extensions
{
    public class HttpContextAuthenticationExtensionsTests
    {
        private readonly HttpContext _context;
        private readonly IServiceCollection _services;
        private readonly MockAuthenticationService _mockAuthService;
        private readonly MockAuthenticationSchemeProvider _mockSchemeProvider;

        public HttpContextAuthenticationExtensionsTests()
        {
            _services = new ServiceCollection();
            _mockAuthService = new MockAuthenticationService();
            _mockSchemeProvider = new MockAuthenticationSchemeProvider();
            
            _services.AddSingleton<IAuthenticationService>(_mockAuthService);
            _services.AddSingleton<IAuthenticationSchemeProvider>(_mockSchemeProvider);
            _services.AddSingleton<ISystemClock>(new SystemClock());
            
            var serviceProvider = _services.BuildServiceProvider();
            _context = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
        }

        [Fact]
        public async Task SignInAsync_WithValidUser_ShouldSucceed()
        {
            // Arrange
            var user = new IdentityServerUser("test_user");
            _services.AddSingleton(new IdentityServerOptions());
            _mockSchemeProvider.DefaultScheme = new AuthenticationScheme("cookies", "cookies", typeof(MockAuthenticationService));

            // Act
            await _context.SignInAsync(user);

            // Assert
            _mockAuthService.SignInCount.Should().Be(1);
            _mockAuthService.SignInScheme.Should().Be("cookies");
        }

        [Fact]
        public async Task SignInAsync_WithProperties_ShouldSucceed()
        {
            // Arrange
            var user = new IdentityServerUser("test_user");
            var props = new AuthenticationProperties();
            _services.AddSingleton(new IdentityServerOptions());
            _mockSchemeProvider.DefaultScheme = new AuthenticationScheme("cookies", "cookies", typeof(MockAuthenticationService));

            // Act
            await _context.SignInAsync(user, props);

            // Assert
            _mockAuthService.SignInCount.Should().Be(1);
            _mockAuthService.SignInScheme.Should().Be("cookies");
        }

        [Fact]
        public async Task GetCookieAuthenticationSchemeAsync_WithConfiguredScheme_ShouldReturnConfiguredScheme()
        {
            // Arrange
            var options = new IdentityServerOptions
            {
                Authentication = new AuthenticationOptions
                {
                    CookieAuthenticationScheme = "custom_scheme"
                }
            };
            _services.AddSingleton(options);

            // Act
            var scheme = await _context.GetCookieAuthenticationSchemeAsync();

            // Assert
            scheme.Should().Be("custom_scheme");
        }

        [Fact]
        public async Task GetCookieAuthenticationSchemeAsync_WithNoScheme_ShouldThrowException()
        {
            // Arrange
            _services.AddSingleton(new IdentityServerOptions());
            _mockSchemeProvider.DefaultScheme = null;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _context.GetCookieAuthenticationSchemeAsync());
        }
    }
}
