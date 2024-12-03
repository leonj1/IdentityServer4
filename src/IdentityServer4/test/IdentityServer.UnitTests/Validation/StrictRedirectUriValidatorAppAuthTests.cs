using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class StrictRedirectUriValidatorAppAuthTests
    {
        private readonly StrictRedirectUriValidatorAppAuth _validator;
        private readonly Client _client;

        public StrictRedirectUriValidatorAppAuthTests()
        {
            _validator = new StrictRedirectUriValidatorAppAuth(new LoggerFactory().CreateLogger<StrictRedirectUriValidatorAppAuth>());
            _client = new Client
            {
                RequirePkce = true,
                RedirectUris = { "http://127.0.0.1" }
            };
        }

        [Theory]
        [InlineData("http://127.0.0.1:1234")]
        [InlineData("http://127.0.0.1:1234/callback")]
        [InlineData("http://127.0.0.1:1234/callback?param=value")]
        [InlineData("http://127.0.0.1:1234/callback#fragment")]
        public async Task Valid_Loopback_Uris_Should_Pass(string uri)
        {
            var result = await _validator.IsRedirectUriValidAsync(uri, _client);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("http://localhost:1234")]
        [InlineData("https://127.0.0.1:1234")]
        [InlineData("http://127.0.0.1:-1")]
        [InlineData("http://127.0.0.1:65536")]
        [InlineData("http://127.0.0.1:invalid")]
        public async Task Invalid_Loopback_Uris_Should_Fail(string uri)
        {
            var result = await _validator.IsRedirectUriValidAsync(uri, _client);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Fail_When_Pkce_Not_Required()
        {
            var client = new Client
            {
                RequirePkce = false,
                RedirectUris = { "http://127.0.0.1" }
            };

            var result = await _validator.IsRedirectUriValidAsync("http://127.0.0.1:1234", client);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Fail_When_127_0_0_1_Not_In_RedirectUris()
        {
            var client = new Client
            {
                RequirePkce = true,
                RedirectUris = { "http://localhost" }
            };

            var result = await _validator.IsRedirectUriValidAsync("http://127.0.0.1:1234", client);
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("http://127.0.0.1:1234")]
        [InlineData("http://127.0.0.1:1234/signout")]
        public async Task Valid_Post_Logout_Loopback_Uris_Should_Pass(string uri)
        {
            var result = await _validator.IsPostLogoutRedirectUriValidAsync(uri, _client);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("http://localhost:1234/signout")]
        public async Task Invalid_Post_Logout_Loopback_Uris_Should_Fail(string uri)
        {
            var result = await _validator.IsPostLogoutRedirectUriValidAsync(uri, _client);
            result.Should().BeFalse();
        }
    }
}
