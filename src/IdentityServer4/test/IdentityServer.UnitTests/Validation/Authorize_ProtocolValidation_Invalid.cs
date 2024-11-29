using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class Authorize_ProtocolValidation_Invalid
    {
        private const string Category = "Authorize Protocol Validation - Invalid";
        private readonly IdentityServerOptions _options;
        private readonly AuthorizeRequestValidator _validator;

        public Authorize_ProtocolValidation_Invalid()
        {
            _options = new IdentityServerOptions();
            _validator = new AuthorizeRequestValidator(_options);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ClientId_Should_Be_Invalid()
        {
            var parameters = new NameValueCollection();
            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
            result.ErrorDescription.Should().Be("client_id is missing");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseType_Should_Be_Invalid()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "response_type", "invalid" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("unsupported_response_type");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Scope_Should_Be_Invalid()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "response_type", "code" },
                { "scope", "" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
            result.ErrorDescription.Should().Be("scope is required");
        }
    }
}
