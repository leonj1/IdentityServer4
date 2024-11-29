using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Services.Default
{
    public class DefaultClaimsServiceTests
    {
        private DefaultClaimsService _subject;
        private Mock<IProfileService> _mockProfileService;
        private Mock<ILogger<DefaultClaimsService>> _mockLogger;
        private ClaimsPrincipal _user;
        private Client _client;

        public DefaultClaimsServiceTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _mockLogger = new Mock<ILogger<DefaultClaimsService>>();
            _subject = new DefaultClaimsService(_mockProfileService.Object, _mockLogger.Object);

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtClaimTypes.Subject, "123"),
                new Claim(JwtClaimTypes.AuthenticationTime, "1234567890"),
                new Claim(JwtClaimTypes.IdentityProvider, "idp"),
                new Claim(JwtClaimTypes.AuthenticationMethod, "method")
            }));

            _client = new Client
            {
                ClientId = "client",
                Claims = new List<ClientClaim>
                {
                    new ClientClaim("client_claim", "client_claim_value")
                }
            };
        }

        [Fact]
        public async Task GetIdentityTokenClaimsAsync_ShouldIncludeStandardSubjectClaims()
        {
            // Arrange
            var resources = new ResourceValidationResult();
            var request = new ValidatedRequest { Client = _client };

            // Act
            var claims = await _subject.GetIdentityTokenClaimsAsync(_user, resources, true, request);

            // Assert
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Subject && c.Value == "123");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.AuthenticationTime && c.Value == "1234567890");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.IdentityProvider && c.Value == "idp");
            claims.Should().Contain(c => c.Type == JwtClaimTypes.AuthenticationMethod && c.Value == "method");
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_ShouldIncludeClientId()
        {
            // Arrange
            var resources = new ResourceValidationResult();
            var request = new ValidatedRequest { Client = _client, ClientId = "client" };

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            claims.Should().Contain(c => c.Type == JwtClaimTypes.ClientId && c.Value == "client");
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_WhenClientHasAlwaysSendClientClaims_ShouldIncludeClientClaims()
        {
            // Arrange
            _client.AlwaysSendClientClaims = true;
            var resources = new ResourceValidationResult();
            var request = new ValidatedRequest 
            { 
                Client = _client, 
                ClientId = "client",
                ClientClaims = _client.Claims
            };

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            claims.Should().Contain(c => c.Type == "client_claim" && c.Value == "client_claim_value");
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_WhenResourceHasUserClaims_ShouldRequestProfileClaims()
        {
            // Arrange
            var apiResource = new ApiResource
            {
                UserClaims = { "api_claim" }
            };
            var resources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiResources = new[] { apiResource }
                }
            };
            var request = new ValidatedRequest { Client = _client };

            _mockProfileService.Setup(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()))
                .Callback<ProfileDataRequestContext>(ctx =>
                {
                    ctx.IssuedClaims = new[] 
                    { 
                        new Claim("api_claim", "api_claim_value") 
                    };
                });

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            claims.Should().Contain(c => c.Type == "api_claim" && c.Value == "api_claim_value");
            _mockProfileService.Verify(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()), Times.Once);
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_ShouldIncludeRequestedScopes()
        {
            // Arrange
            var resources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiScopes = new[] { new ApiScope("api1"), new ApiScope("api2") }
                },
                ParsedScopes = new[] { 
                    new ParsedScopeValue("api1"),
                    new ParsedScopeValue("api2")
                }
            };
            var request = new ValidatedRequest { Client = _client };

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            var scopeClaims = claims.Where(c => c.Type == JwtClaimTypes.Scope).Select(c => c.Value);
            scopeClaims.Should().BeEquivalentTo(new[] { "api1", "api2" });
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_WhenOfflineAccessRequested_ShouldIncludeOfflineScope()
        {
            // Arrange
            var resources = new ResourceValidationResult
            {
                Resources = new Resources { OfflineAccess = true }
            };
            var request = new ValidatedRequest { Client = _client };

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            claims.Should().Contain(c => 
                c.Type == JwtClaimTypes.Scope && 
                c.Value == IdentityServerConstants.StandardScopes.OfflineAccess);
        }

        [Fact]
        public async Task GetAccessTokenClaimsAsync_ShouldFilterProtocolClaims()
        {
            // Arrange
            var resources = new ResourceValidationResult();
            var request = new ValidatedRequest { Client = _client };

            _mockProfileService.Setup(x => x.GetProfileDataAsync(It.IsAny<ProfileDataRequestContext>()))
                .Callback<ProfileDataRequestContext>(ctx =>
                {
                    ctx.IssuedClaims = new[] 
                    { 
                        new Claim("aud", "filtered_value"),
                        new Claim("iss", "filtered_value"),
                        new Claim("normal_claim", "normal_value")
                    };
                });

            // Act
            var claims = await _subject.GetAccessTokenClaimsAsync(_user, resources, request);

            // Assert
            claims.Should().NotContain(c => c.Type == "aud" && c.Value == "filtered_value");
            claims.Should().NotContain(c => c.Type == "iss" && c.Value == "filtered_value");
            claims.Should().Contain(c => c.Type == "normal_claim" && c.Value == "normal_value");
        }
    }
}
