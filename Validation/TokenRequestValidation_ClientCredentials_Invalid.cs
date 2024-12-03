using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenRequestValidation_ClientCredentials_Invalid
    {
        private readonly TokenRequestValidator _validator;
        private readonly Client _client;

        public TokenRequestValidation_ClientCredentials_Invalid()
        {
            _client = new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "api1", "api2" }
            };

            _validator = new TokenRequestValidator();
        }

        [Fact]
        public async Task Invalid_Client_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("client_id", "invalid");
            parameters.Add("client_secret", "secret");
            parameters.Add("scope", "api1");

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Invalid_Client_Secret_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("client_id", "client");
            parameters.Add("client_secret", "invalid");
            parameters.Add("scope", "api1");

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task Invalid_Scope_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("client_id", "client");
            parameters.Add("client_secret", "secret");
            parameters.Add("scope", "invalid");

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_scope");
        }

        [Fact]
        public async Task Missing_Grant_Type_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("client_id", "client");
            parameters.Add("client_secret", "secret");
            parameters.Add("scope", "api1");

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_request");
        }
    }
}
