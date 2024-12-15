using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class TokenValidationTests
{
    [Fact]
    public async Task Non_existing_RefreshToken_Request()
    {
        var client = await Clients.FindClientByIdAsync("roclient");
        var validator = new TokenRequestValidator(
            Clients,
            Resources,
            Profile);

        var parameters = new NameValueCollection();
        parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
        parameters.Add(OidcConstants.TokenRequest.RefreshToken, "non_existing_handle");

        var result = await validator.ValidateAsync(parameters, client.ToValidationParameters());

        result.IsError.Should().BeTrue();
        result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
    }
}
