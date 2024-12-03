using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class Authorize_ProtocolValidation_PKCE
    {
        private const string TestCodeVerifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
        private readonly IdentityServerOptions _options;
        private readonly AuthorizeRequestValidator _validator;

        public Authorize_ProtocolValidation_PKCE()
        {
            _options = new IdentityServerOptions();
            _validator = new AuthorizeRequestValidator(_options);
        }

        [Fact]
        public async Task Valid_CodeChallenge_Plain()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                CodeChallenge = TestCodeVerifier,
                CodeChallengeMethod = "plain",
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Valid_CodeChallenge_SHA256()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                CodeChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM",
                CodeChallengeMethod = "S256",
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Missing_CodeChallenge_When_Required()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }

        [Fact]
        public async Task Invalid_CodeChallengeMethod()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                CodeChallenge = TestCodeVerifier,
                CodeChallengeMethod = "invalid_method",
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }

        [Fact]
        public async Task CodeChallenge_TooShort()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                CodeChallenge = "short",
                CodeChallengeMethod = "S256",
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }

        [Fact]
        public async Task CodeChallenge_TooLong()
        {
            var parameters = new ValidatedAuthorizeRequest
            {
                CodeChallenge = new string('a', 129),
                CodeChallengeMethod = "S256",
                Client = new Client { RequirePkce = true }
            };

            var result = await _validator.ValidateCodeChallengeAsync(parameters);
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }
    }
}
