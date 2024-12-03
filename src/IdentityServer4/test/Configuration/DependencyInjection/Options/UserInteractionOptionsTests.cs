using System;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration
{
    public class UserInteractionOptionsTests
    {
        private UserInteractionOptions _options = new UserInteractionOptions();

        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Verify default values match Constants
            _options.LoginUrl.Should().Be(Constants.UIConstants.DefaultRoutePaths.Login.EnsureLeadingSlash());
            _options.LogoutUrl.Should().Be(Constants.UIConstants.DefaultRoutePaths.Logout.EnsureLeadingSlash());
            _options.ConsentUrl.Should().Be(Constants.UIConstants.DefaultRoutePaths.Consent.EnsureLeadingSlash());
            _options.ErrorUrl.Should().Be(Constants.UIConstants.DefaultRoutePaths.Error.EnsureLeadingSlash());
            _options.DeviceVerificationUrl.Should().Be(Constants.UIConstants.DefaultRoutePaths.DeviceVerification);
        }

        [Fact]
        public void DefaultParameters_ShouldBeCorrect()
        {
            _options.LogoutIdParameter.Should().Be(Constants.UIConstants.DefaultRoutePathParams.Logout);
            _options.ConsentReturnUrlParameter.Should().Be(Constants.UIConstants.DefaultRoutePathParams.Consent);
            _options.ErrorIdParameter.Should().Be(Constants.UIConstants.DefaultRoutePathParams.Error);
            _options.CustomRedirectReturnUrlParameter.Should().Be(Constants.UIConstants.DefaultRoutePathParams.Custom);
            _options.DeviceVerificationUserCodeParameter.Should().Be(Constants.UIConstants.DefaultRoutePathParams.UserCode);
        }

        [Fact]
        public void CookieMessageThreshold_ShouldHaveCorrectDefault()
        {
            _options.CookieMessageThreshold.Should().Be(Constants.UIConstants.CookieMessageThreshold);
        }

        [Theory]
        [InlineData("/login")]
        [InlineData("/custom/login")]
        public void LoginUrl_WhenSet_ShouldAcceptValidValues(string url)
        {
            _options.LoginUrl = url;
            _options.LoginUrl.Should().Be(url);
        }

        [Theory]
        [InlineData("login")]
        [InlineData("invalid")]
        public void LoginUrl_WhenSetWithoutLeadingSlash_ShouldThrowException(string url)
        {
            Action act = () => _options.LoginUrl = url;
            act.Should().Throw<ArgumentException>();
        }
    }
}
