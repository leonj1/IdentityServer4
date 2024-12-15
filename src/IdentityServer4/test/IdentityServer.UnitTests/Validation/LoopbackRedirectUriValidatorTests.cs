using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class LoopbackRedirectUriValidatorTests
    {
        private const string Category = "Loopback Redirect Uri Validator Tests";

        [Theory]
        [Trait("Category", Category)]
        [InlineData("http://127.0.0.1")] // This is in the clients redirect URIs
        [InlineData("http://127.0.0.1:0")]
        [InlineData("http://127.0.0.1:80")]
        [InlineData("http://127.0.0.1:65535")]
        [InlineData("http://127.0.0.1:123/a/b")]
        [InlineData("http://127.0.0.1:123?q=123")]
        [InlineData("http://127.0.0.1:443/?q=123")]
        [InlineData("http://127.0.0.1:443/a/b?q=123")]
        [InlineData("http://127.0.0.1#abc")]
        public async Task Loopback_Redirect_URIs_Should_Be_AllowedAsync(string requestedUri)
        {
            var strictRedirectUriValidatorAppAuthValidator = new StrictRedirectUriValidatorAppAuth(TestLogger.Create<StrictRedirectUriValidatorAppAuth>());

            var result = await strictRedirectUriValidatorAppAuthValidator.IsRedirectUriValidAsync(requestedUri, clientWithValidLoopbackRedirectUri);

            result.Should().BeTrue();
        }
    }
}
