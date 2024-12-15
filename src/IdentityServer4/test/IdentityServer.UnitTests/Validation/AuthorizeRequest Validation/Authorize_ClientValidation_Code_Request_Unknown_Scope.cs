using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer.UnitTests.Validation.AuthorizeRequest_Validation
{
    public class Authorize_ClientValidation_Code_Request_Unknown_Scope
    {
        private IdentityServerOptions _options = TestIdentityServerOptions.Create();

        [Fact]
        [Trait("Category", "AuthorizeRequest Client Validation - Code")]
        public async Task Code_Request_Unknown_Scope()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "unknown");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);
            
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidScope);
        }
    }
}
