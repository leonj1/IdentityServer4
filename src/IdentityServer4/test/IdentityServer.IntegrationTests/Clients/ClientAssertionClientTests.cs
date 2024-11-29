using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class ClientAssertionClientTests
    {
        private readonly ClientAssertionClient _sut;

        public ClientAssertionClientTests()
        {
            _sut = new ClientAssertionClient();
        }

        [Fact]
        public async Task When_TokenRequestIsValid_Should_ReturnValidToken()
        {
            // Arrange
            var token = _sut.CreateToken("certificate_base64_valid");
            
            // Act
            var response = await _sut._client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://idsvr4/connect/token",
                ClientId = "certificate_base64_valid",
                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = token
                },
                Scope = "api1"
            });

            // Assert
            response.IsError.Should().BeFalse();
            response.ExpiresIn.Should().Be(3600);
            response.TokenType.Should().Be("Bearer");
            response.IdentityToken.Should().BeNull();
            response.RefreshToken.Should().BeNull();
        }

        [Fact]
        public async Task When_TokenIsReplayed_Should_ReturnError()
        {
            // Arrange
            var token = _sut.CreateToken("certificate_base64_valid");
            var request = new ClientCredentialsTokenRequest
            {
                Address = "https://idsvr4/connect/token",
                ClientId = "certificate_base64_valid",
                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = token
                },
                Scope = "api1"
            };

            // Act
            var firstResponse = await _sut._client.RequestClientCredentialsTokenAsync(request);
            var replayResponse = await _sut._client.RequestClientCredentialsTokenAsync(request);

            // Assert
            firstResponse.IsError.Should().BeFalse();
            replayResponse.IsError.Should().BeTrue();
            replayResponse.Error.Should().Be("invalid_client");
        }

        [Fact]
        public async Task When_InvalidClientSecret_Should_ReturnError()
        {
            // Arrange & Act
            var response = await _sut._client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://idsvr4/connect/token",
                ClientId = "certificate_base64_valid",
                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = "invalid"
                },
                Scope = "api1"
            });

            // Assert
            response.IsError.Should().BeTrue();
            response.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
        }

        [Fact]
        public async Task When_InvalidClientId_Should_ReturnError()
        {
            // Arrange
            var token = _sut.CreateToken("certificate_base64_invalid");

            // Act
            var response = await _sut._client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://idsvr4/connect/token",
                ClientId = "certificate_base64_invalid",
                ClientAssertion = new ClientAssertion
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = token
                },
                Scope = "api1"
            });

            // Assert
            response.IsError.Should().BeTrue();
            response.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
        }
    }
}
