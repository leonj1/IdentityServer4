using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.UnitTests.Common;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class Authorize_ClientValidation_Token
    {
        private const string Category = "Authorize Client Validation - Token";
        private IClientConfigurationValidator _validator;
        private Client _client;

        public Authorize_ClientValidation_Token()
        {
            _client = new Client
            {
                ClientId = "client",
                Enabled = true,
                RequireClientSecret = true,
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowedScopes = { "scope1", "scope2" }
            };

            _validator = new DefaultClientConfigurationValidator(TestLogger.Create<DefaultClientConfigurationValidator>());
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Client_Should_Pass_Validation()
        {
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task DisabledClient_Should_Fail_Validation()
        {
            _client.Enabled = false;
            
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Contain("disabled");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_GrantType_Should_Fail_Validation()
        {
            _client.AllowedGrantTypes = new List<string> { "invalid_grant" };
            
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Contain("grant type");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Scopes_Should_Fail_Validation()
        {
            _client.AllowedScopes = new List<string>();
            
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
            context.ErrorMessage.Should().Contain("scopes");
        }
    }
}
