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
    public class Authorize_ClientValidation_Code
    {
        private const string Category = "Authorize Client Validation - Code";
        private IClientConfigurationValidator _validator;
        private Client _client;

        public Authorize_ClientValidation_Code()
        {
            _validator = new DefaultClientConfigurationValidator(TestLogger.Create<DefaultClientConfigurationValidator>());
            _client = new Client
            {
                ClientId = "client",
                Enabled = true,
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://client.com/callback" },
                AllowedScopes = { "openid", "profile" }
            };
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Standard_Code_Client_Should_Be_Valid()
        {
            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Client_Without_RedirectUri_Should_Be_Invalid()
        {
            _client.RedirectUris.Clear();

            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Client_Without_AllowedScopes_Should_Be_Invalid()
        {
            _client.AllowedScopes.Clear();

            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Disabled_Client_Should_Be_Invalid()
        {
            _client.Enabled = false;

            var context = new ClientConfigurationValidationContext(_client);
            await _validator.ValidateAsync(context);

            context.IsValid.Should().BeFalse();
        }
    }
}
