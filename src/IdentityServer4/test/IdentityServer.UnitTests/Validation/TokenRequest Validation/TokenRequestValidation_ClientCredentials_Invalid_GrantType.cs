using FluentAssertions;
using Xunit;

public class TokenRequestValidation_ClientCredentials_Invalid_GrantType
{
    [Fact]
    public async Task Invalid_GrantType_For_Client()
    {
        var client = await _clients.FindEnabledClientByIdAsync("client");
        var validator = Factory.CreateTokenRequestValidator();

        var parameters = new NameValueCollection();
        parameters.Add(OidcConstants.TokenRequest.GrantType, "invalid_grant_type");
        parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

        var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

        result.IsError.Should().BeTrue();
        result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
    }
}
