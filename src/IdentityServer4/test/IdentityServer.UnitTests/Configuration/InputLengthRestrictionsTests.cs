using IdentityServer4.Configuration;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Configuration
{
    public class InputLengthRestrictionsTests
    {
        private InputLengthRestrictions _restrictions;

        public InputLengthRestrictionsTests()
        {
            _restrictions = new InputLengthRestrictions();
        }

        [Fact]
        public void Default_values_should_be_correct()
        {
            // Default value tests
            _restrictions.ClientId.Should().Be(100);
            _restrictions.ClientSecret.Should().Be(100);
            _restrictions.Scope.Should().Be(300);
            _restrictions.RedirectUri.Should().Be(400);
            _restrictions.Nonce.Should().Be(300);
            _restrictions.UiLocale.Should().Be(100);
            _restrictions.LoginHint.Should().Be(100);
            _restrictions.AcrValues.Should().Be(300);
            _restrictions.GrantType.Should().Be(100);
            _restrictions.UserName.Should().Be(100);
            _restrictions.Password.Should().Be(100);
            _restrictions.CspReport.Should().Be(2000);
            _restrictions.IdentityProvider.Should().Be(100);
            _restrictions.ExternalError.Should().Be(100);
            _restrictions.AuthorizationCode.Should().Be(100);
            _restrictions.DeviceCode.Should().Be(100);
            _restrictions.RefreshToken.Should().Be(100);
            _restrictions.TokenHandle.Should().Be(100);
            _restrictions.Jwt.Should().Be(51200);
        }

        [Fact]
        public void Code_challenge_values_should_be_correct()
        {
            _restrictions.CodeChallengeMinLength.Should().Be(43);
            _restrictions.CodeChallengeMaxLength.Should().Be(128);
        }

        [Fact]
        public void Code_verifier_values_should_be_correct()
        {
            _restrictions.CodeVerifierMinLength.Should().Be(43);
            _restrictions.CodeVerifierMaxLength.Should().Be(128);
        }

        [Fact]
        public void Properties_should_be_settable()
        {
            _restrictions.ClientId = 200;
            _restrictions.ClientId.Should().Be(200);

            _restrictions.Scope = 400;
            _restrictions.Scope.Should().Be(400);

            _restrictions.RedirectUri = 500;
            _restrictions.RedirectUri.Should().Be(500);
        }
    }
}
