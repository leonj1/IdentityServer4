using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class IdentityServerAuthenticationServiceTests
    {
        private readonly Mock<IAuthenticationService> _mockInner;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IUserSession> _mockUserSession;
        private readonly Mock<IBackChannelLogoutService> _mockBackChannelLogoutService;
        private readonly IdentityServerOptions _options;
        private readonly Mock<ILogger<IdentityServerAuthenticationService>> _mockLogger;
        private readonly IdentityServerAuthenticationService _service;

        public IdentityServerAuthenticationServiceTests()
        {
            _mockInner = new Mock<IAuthenticationService>();
            _mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _mockClock = new Mock<ISystemClock>();
            _mockUserSession = new Mock<IUserSession>();
            _mockBackChannelLogoutService = new Mock<IBackChannelLogoutService>();
            _options = new IdentityServerOptions();
            _mockLogger = new Mock<ILogger<IdentityServerAuthenticationService>>();

            var decorator = new Decorator<IAuthenticationService>(_mockInner.Object);

            _service = new IdentityServerAuthenticationService(
                decorator,
                _mockSchemeProvider.Object,
                _mockClock.Object,
                _mockUserSession.Object,
                _mockBackChannelLogoutService.Object,
                _options,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task SignInAsync_WhenValidPrincipal_ShouldSucceed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";
            var expectedResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), scheme));

            _mockInner.Setup(x => x.AuthenticateAsync(context, scheme))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.SignInAsync(context, scheme, new ClaimsPrincipal());

            // Assert
            Assert.Same(expectedResult, result);
        }

        [Fact]
        public async Task SignOutAsync_ShouldSetSignOutFlag()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";

            _mockSchemeProvider.Setup(x => x.GetDefaultSignOutSchemeAsync())
                .ReturnsAsync(new AuthenticationScheme(scheme, scheme, typeof(IAuthenticationHandler)));

            // Act
            await _service.SignOutAsync(context, scheme);

            // Assert
            _mockInner.Verify(x => x.SignOutAsync(context, scheme), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldDelegateToInnerService()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";
            var expectedResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), scheme));

            _mockInner.Setup(x => x.AuthenticateAsync(context, scheme))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.AuthenticateAsync(context, scheme);

            // Assert
            Assert.Same(expectedResult, result);
        }
    }
}
