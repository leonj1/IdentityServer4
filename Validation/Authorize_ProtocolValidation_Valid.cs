using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class Authorize_ProtocolValidation_Valid
    {
        private const string Category = "Authorize Protocol Validation - Valid";
        private readonly IdentityServerOptions _options;
        private readonly AuthorizeRequestValidator _validator;

        public Authorize_ProtocolValidation_Valid()
        {
            _options = new IdentityServerOptions();
            _validator = new AuthorizeRequestValidator(_options);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_OpenId_Code_Request()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "scope", "openid" },
                { "response_type", "code" },
                { "redirect_uri", "https://client/callback" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Resource_Code_Request()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "scope", "resource1" },
                { "response_type", "code" },
                { "redirect_uri", "https://client/callback" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Mixed_Code_Request()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "scope", "openid resource1 resource2" },
                { "response_type", "code" },
                { "redirect_uri", "https://client/callback" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Mixed_Code_Request_with_State()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "scope", "openid resource1 resource2" },
                { "response_type", "code" },
                { "redirect_uri", "https://client/callback" },
                { "state", "abc" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_OpenId_IdToken_Request()
        {
            var parameters = new NameValueCollection
            {
                { "client_id", "client" },
                { "scope", "openid" },
                { "response_type", "id_token" },
                { "redirect_uri", "https://client/callback" },
                { "nonce", "abc" }
            };

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }
    }
}
