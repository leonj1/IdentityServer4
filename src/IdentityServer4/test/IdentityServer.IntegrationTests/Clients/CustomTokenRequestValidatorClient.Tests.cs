using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;
using Moq;
using Microsoft.AspNetCore.TestHost;

namespace IdentityServer.IntegrationTests.Clients
{
    public class CustomTokenRequestValidatorClientTests
    {
        private readonly CustomTokenRequestValidatorClient _sut;

        public CustomTokenRequestValidatorClientTests()
        {
            _sut = new CustomTokenRequestValidatorClient();
        }

        [Fact]
        public async Task Client_credentials_request_should_return_success_response()
        {
            // Act
            var response = await _sut._client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://server/connect/token",
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            // Assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Resource_owner_credentials_request_should_return_success_response()
        {
            // Act
            var response = await _sut._client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = "https://server/connect/token",
                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "api1",
                UserName = "bob",
                Password = "bob"
            });

            // Assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Extension_grant_request_should_return_success_response()
        {
            // Act
            var response = await _sut._client.RequestTokenAsync(new TokenRequest
            {
                Address = "https://server/connect/token",
                GrantType = "custom",
                ClientId = "client.custom",
                ClientSecret = "secret",
                Parameters =
                {
                    { "scope", "api1" },
                    { "custom_credential", "custom credential"}
                }
            });

            // Assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task GetFields_should_return_dictionary_with_custom_field()
        {
            // Arrange
            var response = await _sut._client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://server/connect/token",
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            // Act
            var fields = _sut.GetFields(response);

            // Assert
            fields.Should().ContainKey("custom");
            fields["custom"].Should().Be("custom");
        }
    }
}
