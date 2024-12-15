using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
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
        private readonly Mock<IAuthenticationService> _mockInnerService;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IUserSession> _mockUserSession;
        private readonly Mock<IBackChannelLogoutService> _mockBackChannelLogoutService;
        private readonly IdentityServerOptions _options;
        private readonly Mock<ILogger<IdentityServerAuthenticationService>> _mockLogger;
        private readonly IdentityServerAuthenticationService _subject;

        public IdentityServerAuthenticationServiceTests()
        {
            _mockInnerService = new Mock<IAuthenticationService>();
            _mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _mockClock = new Mock<ISystemClock>();
            _mockUserSession = new Mock<IUserSession>();
            _mockBackChannelLogoutService = new Mock<IBackChannelLogoutService>();
            _options = new IdentityServerOptions();
            _mockLogger = new Mock<ILogger<IdentityServerAuthenticationService>>();

            var decorator = new Decorator<IAuthenticationService>(_mockInnerService.Object);

            _subject = new IdentityServerAuthenticationService(
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
        public async Task SignInAsync_ShouldSetSubClaim()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtRegisteredClaimNames.Sub, "user123") }));
            var properties = new AuthenticationProperties();

            _mockSchemeProvider.Setup(x => x.GetDefaultSignInSchemeAsync())
                .ReturnsAsync(new AuthenticationScheme(scheme, scheme, typeof(IAuthenticationHandler)));

            // Act
            await _subject.SignInAsync(context, scheme, principal, properties);

            // Assert
            _mockInnerService.Verify(x => x.SignInAsync(context, scheme, It.Is<ClaimsPrincipal>(p => p.HasClaim(JwtRegisteredClaimNames.Sub, "user123")), properties), Times.Once);
        }

        [Fact]
        public async Task SignOutAsync_ShouldDelegateToInnerService()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";
            var expectedResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), scheme));

            _mockInnerService.Setup(x => x.AuthenticateAsync(context, scheme))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _subject.AuthenticateAsync(context, scheme);

            // Assert
            result.Should().Be(expectedResult);
            _mockInnerService.Verify(x => x.AuthenticateAsync(context, scheme), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldDelegateToInnerService()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "cookies";
            var expectedResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), scheme));

            _mockInnerService.Setup(x => x.AuthenticateAsync(context, scheme))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _subject.AuthenticateAsync(context, scheme);

            // Assert
            result.Should().Be(expectedResult);
            _mockInnerService.Verify(x => x.AuthenticateAsync(context, scheme), Times.Once);
        }
    }
}
