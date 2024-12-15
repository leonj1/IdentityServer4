using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TokenValidationTests
{
    public class NonExistingRefreshTokenTest : IClassFixture<TokenValidationTestFixture>
    {
        private readonly TokenValidationTestFixture _fixture;

        public NonExistingRefreshTokenTest(TokenValidationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_return_invalid_grant_error_for_non_existing_refresh_token()
        {
            var client = await _fixture.Clients.GetAsync("roclient");
            var parameters = new NameValueCollection
            {
                { OidcConstants.TokenRequest.GrantType, "refresh_token" },
                { OidcConstants.TokenRequest.RefreshToken, "non-existing-token" }
            };

            var result = await _fixture.Validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }
    }
}
