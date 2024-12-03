using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class Authorize_ClientValidation_IdToken
    {
        private const string Category = "Authorize Client Validation - ID Token";
        private readonly IdentityServerOptions _options;
        private readonly Client _client;
        private readonly AuthorizeRequestValidator _validator;

        public Authorize_ClientValidation_IdToken()
        {
            _options = new IdentityServerOptions();
            _client = new Client 
            { 
                ClientId = "client",
                Enabled = true,
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowedScopes = { "openid" }
            };

            _validator = new AuthorizeRequestValidator(
                // Add required dependencies
            );
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_IdToken_Request_Should_Succeed()
        {
            var parameters = new NameValueCollection();
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("response_type", "id_token");
            parameters.Add("scope", "openid");
            parameters.Add("nonce", "abc");

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Nonce_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("response_type", "id_token");
            parameters.Add("scope", "openid");

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseType_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("response_type", "invalid");
            parameters.Add("scope", "openid");
            parameters.Add("nonce", "abc");

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("unsupported_response_type");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_OpenId_Scope_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("response_type", "id_token");
            parameters.Add("scope", "profile");
            parameters.Add("nonce", "abc");

            var result = await _validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_scope");
        }
    }
}
