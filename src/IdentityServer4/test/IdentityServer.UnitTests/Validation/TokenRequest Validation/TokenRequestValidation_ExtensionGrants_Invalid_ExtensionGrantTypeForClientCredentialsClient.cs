using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Validation.TokenRequest_Validation
{
    public class TokenRequestValidation_ExtensionGrants_Invalid_ExtensionGrantTypeForClientCredentialsClient
    {
        private const string Category = "TokenRequest Validation - Extension Grants - Invalid";

        private IClientStore _clients = Factory.CreateClientStore();

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Extension_Grant_Type_For_Client_Credentials_Client()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection
            {
                { OidcConstants.TokenRequest.GrantType, "customGrant" },
                { OidcConstants.TokenRequest.Scope, "resource" }
            };

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
        }
    }
}
