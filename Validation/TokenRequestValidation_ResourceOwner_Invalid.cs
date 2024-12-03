using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenRequestValidation_ResourceOwner_Invalid
    {
        private readonly TokenRequestValidator _validator;
        private readonly Client _client;

        public TokenRequestValidation_ResourceOwner_Invalid()
        {
            _client = new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = new[] { new Secret("secret".Sha256()) },
                AllowedScopes = { "api1" }
            };

            _validator = new TokenRequestValidator();
        }

        [Fact]
        public async Task Missing_Username_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "password");
            parameters.Add("password", "password");

            var result = await _validator.ValidateRequestAsync(
                parameters,
                _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Missing_Password_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "password");
            parameters.Add("username", "user");

            var result = await _validator.ValidateRequestAsync(
                parameters,
                _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_grant");
        }

        [Fact]
        public async Task Invalid_Resource_Owner_Credentials_Should_Fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "password");
            parameters.Add("username", "unknown");
            parameters.Add("password", "invalid");

            var result = await _validator.ValidateRequestAsync(
                parameters, 
                _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_grant");
        }
    }
}
