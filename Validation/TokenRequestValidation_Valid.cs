using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenRequestValidation_Valid
    {
        private readonly TokenRequestValidator _validator;
        private readonly Client _client;

        public TokenRequestValidation_Valid()
        {
            _validator = new TokenRequestValidator();
            _client = new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                RequireClientSecret = true
            };
        }

        [Fact]
        public async Task Valid_ClientCredentials_Request_Should_Succeed()
        {
            // Arrange
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("client_secret", "secret");

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.GrantType.Should().Be("client_credentials");
        }

        [Fact]
        public async Task Valid_ResourceOwner_Request_Should_Succeed()
        {
            // Arrange
            _client.AllowedGrantTypes = GrantTypes.ResourceOwnerPassword;
            
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "password");
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("client_secret", "secret");
            parameters.Add("username", "user");
            parameters.Add("password", "pass");

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.GrantType.Should().Be("password");
        }

        [Fact]
        public async Task Valid_AuthorizationCode_Request_Should_Succeed()
        {
            // Arrange
            _client.AllowedGrantTypes = GrantTypes.Code;
            
            var parameters = new NameValueCollection();
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("client_id", _client.ClientId);
            parameters.Add("client_secret", "secret");
            parameters.Add("code", "valid_code");
            parameters.Add("redirect_uri", "https://client.example.com/callback");

            // Act
            var result = await _validator.ValidateRequestAsync(parameters, _client);

            // Assert
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.GrantType.Should().Be("authorization_code");
        }
    }
}
