using System;
using FluentAssertions;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.UnitTests.Configuration
{
    public class ConfigureInternalCookieOptionsTests
    {
        private readonly IdentityServerOptions _options;
        private readonly CookieAuthenticationOptions _cookieOptions;
        private readonly ConfigureInternalCookieOptions _subject;

        public ConfigureInternalCookieOptionsTests()
        {
            _options = new IdentityServerOptions();
            _cookieOptions = new CookieAuthenticationOptions();
            _subject = new ConfigureInternalCookieOptions(_options);
        }

        [Fact]
        public void Configure_Should_SetDefaultCookieOptions()
        {
            _subject.Configure(IdentityServerConstants.DefaultCookieAuthenticationScheme, _cookieOptions);

            _cookieOptions.SlidingExpiration.Should().Be(_options.Authentication.CookieSlidingExpiration);
            _cookieOptions.ExpireTimeSpan.Should().Be(_options.Authentication.CookieLifetime);
            _cookieOptions.Cookie.Name.Should().Be(IdentityServerConstants.DefaultCookieAuthenticationScheme);
            _cookieOptions.Cookie.IsEssential.Should().BeTrue();
            _cookieOptions.Cookie.SameSite.Should().Be(_options.Authentication.CookieSameSiteMode);
        }

        [Fact]
        public void Configure_Should_SetExternalCookieOptions()
        {
            _subject.Configure(IdentityServerConstants.ExternalCookieAuthenticationScheme, _cookieOptions);

            _cookieOptions.Cookie.Name.Should().Be(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            _cookieOptions.Cookie.IsEssential.Should().BeTrue();
            _cookieOptions.Cookie.SameSite.Should().Be(_options.Authentication.CookieSameSiteMode);
        }

        [Fact]
        public void Configure_Should_HandleLocalUrls()
        {
            _options.UserInteraction.LoginUrl = "~/login";
            _options.UserInteraction.LogoutUrl = "~/logout";

            _subject.Configure(IdentityServerConstants.DefaultCookieAuthenticationScheme, _cookieOptions);

            _cookieOptions.LoginPath.Should().Be("/login");
            _cookieOptions.LogoutPath.Should().Be("/logout");
        }
    }

    public class PostConfigureInternalCookieOptionsTests
    {
        private readonly IdentityServerOptions _idsrvOptions;
        private readonly AuthenticationOptions _authOptions;
        private readonly CookieAuthenticationOptions _cookieOptions;
        private readonly PostConfigureInternalCookieOptions _subject;

        public PostConfigureInternalCookieOptionsTests()
        {
            _idsrvOptions = new IdentityServerOptions();
            _authOptions = new AuthenticationOptions();
            _cookieOptions = new CookieAuthenticationOptions();

            var authOptionsMonitor = new OptionsMonitor<AuthenticationOptions>(
                new OptionsFactory<AuthenticationOptions>(
                    new[] { new ConfigureOptions<AuthenticationOptions>(o => _authOptions = o) },
                    new[]{ new PostConfigureOptions<AuthenticationOptions>(null, o => { }) }
                ),
                new[] { new OptionsChangeTokenSource<AuthenticationOptions>(null) },
                new OptionsCache<AuthenticationOptions>()
            );

            _subject = new PostConfigureInternalCookieOptions(
                _idsrvOptions,
                Options.Create(_authOptions),
                new LoggerFactory()
            );
        }

        [Fact]
        public void PostConfigure_Should_SetDefaultOptions()
        {
            _cookieOptions.LoginPath = "/login";
            _cookieOptions.LogoutPath = "/logout";
            _cookieOptions.ReturnUrlParameter = "returnUrl";

            _authOptions.DefaultScheme = "cookies";
            
            _subject.PostConfigure("cookies", _cookieOptions);

            _idsrvOptions.UserInteraction.LoginUrl.Should().Be(_cookieOptions.LoginPath);
            _idsrvOptions.UserInteraction.LogoutUrl.Should().Be(_cookieOptions.LogoutPath);
            _idsrvOptions.UserInteraction.LoginReturnUrlParameter.Should().Be(_cookieOptions.ReturnUrlParameter);
        }
    }
}
