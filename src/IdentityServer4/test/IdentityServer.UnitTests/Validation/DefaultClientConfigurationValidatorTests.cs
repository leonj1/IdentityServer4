using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class DefaultClientConfigurationValidatorTests
    {
        private readonly IdentityServerOptions _options;
        private readonly DefaultClientConfigurationValidator _validator;
        private readonly Client _client;

        public DefaultClientConfigurationValidatorTests()
        {
            _options = new IdentityServerOptions();
            _validator = new DefaultClientConfigurationValidator(_options);
            _client = new Client
            {
                ClientId = "test-client",
                ClientName = "Test Client",
                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequireClientSecret = true
            };
        }

        [Fact]
        public async Task Valid_Client_Should_Pass_Validation()
        {
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);
            context.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Client_Without_GrantTypes_Should_Fail()
        {
            _client.AllowedGrantTypes.Clear();
            var context = new ClientConfigurationValidationContext(_client);
            
            await _validator.ValidateAsync(context);
            
            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Be("no allowed grant type specified");
        }

        [Fact]
        public async Task Client_With_Invalid_AccessTokenLifetime_Should_Fail()
        {
            _client.AccessTokenLifetime = -1;
            var context = new ClientConfigurationValidationContext(_client);
            
            await _validator.ValidateAsync(context);
            
            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Be("access token lifetime is 0 or negative");
        }

        [Fact]
        public async Task Client_Without_RedirectUri_For_AuthorizationCode_Should_Fail()
        {
            _client.AllowedGrantTypes = GrantTypes.Code;
            _client.RedirectUris.Clear();
            var context = new ClientConfigurationValidationContext(_client);
            
            await _validator.ValidateAsync(context);
            
            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Be("No redirect URI configured.");
        }

        [Fact]
        public async Task Client_Without_Secret_For_ClientCredentials_Should_Fail()
        {
            _client.ClientSecrets.Clear();
            var context = new ClientConfigurationValidationContext(_client);
            
            await _validator.ValidateAsync(context);
            
            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Be("Client secret is required for client_credentials, but no client secret is configured.");
        }

        [Fact]
        public async Task Client_With_Invalid_CorsOrigin_Should_Fail()
        {
            _client.AllowedCorsOrigins = new[] { "invalid-uri" };
            var context = new ClientConfigurationValidationContext(_client);
            
            await _validator.ValidateAsync(context);
            
            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Be("AllowedCorsOrigins contains invalid origin: invalid-uri");
        }
    }
}
