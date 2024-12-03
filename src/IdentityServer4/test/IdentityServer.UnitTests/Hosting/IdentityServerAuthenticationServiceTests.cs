using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace IdentityServer.UnitTests.Hosting
{
    public class IdentityServerAuthenticationServiceTests
    {
        private readonly IAuthenticationService _inner;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly ISystemClock _clock;
        private readonly IUserSession _session;
        private readonly IBackChannelLogoutService _backChannelLogoutService;
        private readonly IdentityServerOptions _options;
        private readonly ILogger<IdentityServer4.Hosting.IdentityServerAuthenticationService> _logger;
        private readonly IdentityServer4.Hosting.IdentityServerAuthenticationService _subject;

        public IdentityServerAuthenticationServiceTests()
        {
            _inner = Substitute.For<IAuthenticationService>();
            _schemes = Substitute.For<IAuthenticationSchemeProvider>();
            _clock = Substitute.For<ISystemClock>();
            _session = Substitute.For<IUserSession>();
            _backChannelLogoutService = Substitute.For<IBackChannelLogoutService>();
            _options = new IdentityServerOptions();
            _logger = Substitute.For<ILogger<IdentityServer4.Hosting.IdentityServerAuthenticationService>>();

            var decorator = new Decorator<IAuthenticationService>(_inner);
            _subject = new IdentityServer4.Hosting.IdentityServerAuthenticationService(
                decorator,
                _schemes,
                _clock,
                _session,
                _backChannelLogoutService,
                _options,
                _logger
            );
        }

        [Fact]
        public async Task SignInAsync_when_no_sub_claim_should_throw()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            var scheme = "scheme";
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            var props = new AuthenticationProperties();

            // Act
            Func<Task> act = () => _subject.SignInAsync(ctx, scheme, principal, props);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("sub claim is missing");
        }

        [Fact]
        public async Task SignInAsync_when_multiple_identities_should_throw()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            var scheme = "scheme";
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity[] { 
                    new ClaimsIdentity(),
                    new ClaimsIdentity()
                });
            var props = new AuthenticationProperties();

            // Act
            Func<Task> act = () => _subject.SignInAsync(ctx, scheme, principal, props);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("only a single identity supported");
        }

        [Fact]
        public async Task SignInAsync_should_add_required_claims()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            var scheme = "scheme";
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(JwtClaimTypes.Subject, "123"));
            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties();

            var utcNow = DateTime.UtcNow;
            _clock.UtcNow.Returns(new DateTimeOffset(utcNow));

            // Act
            await _subject.SignInAsync(ctx, scheme, principal, props);

            // Assert
            principal.FindFirst(JwtClaimTypes.IdentityProvider).Value.Should().Be(IdentityServerConstants.LocalIdentityProvider);
            principal.FindFirst(JwtClaimTypes.AuthenticationMethod).Value.Should().Be(OidcConstants.AuthenticationMethods.Password);
            principal.FindFirst(JwtClaimTypes.AuthenticationTime).Value.Should()
                .Be(new DateTimeOffset(utcNow).ToUnixTimeSeconds().ToString());
        }

        [Fact]
        public async Task SignOutAsync_should_set_signout_flag()
        {
            // Arrange
            var ctx = new DefaultHttpContext();
            var scheme = "scheme";
            var props = new AuthenticationProperties();

            _schemes.GetDefaultSignOutSchemeAsync().Returns(new AuthenticationScheme(scheme, scheme, typeof(IAuthenticationHandler)));
            ctx.GetCookieAuthenticationSchemeAsync().Returns(scheme);

            // Act
            await _subject.SignOutAsync(ctx, scheme, props);

            // Assert
            ctx.GetSignOutCalled().Should().BeTrue();
        }
    }
}
