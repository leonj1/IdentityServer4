using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Configuration;
using Xunit;

namespace IdentityServer.UnitTests.Validation.AuthorizeRequest_Validation
{
    public class MixedTokenRequestValidation
    {
        [Fact]
        public async Task Mixed_Token_Request_Without_OpenId_Scope()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "resource profile");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Token);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);
            
            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidScope);
        }
    }
}
