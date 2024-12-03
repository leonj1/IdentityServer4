using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Xunit;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class TokenResponseGeneratorTests
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IScopeParser _scopeParser;
        private readonly IResourceStore _resources;
        private readonly IClientStore _clients;
        private readonly ISystemClock _clock;
        private readonly ILogger<TokenResponseGenerator> _logger;
        private readonly TokenResponseGenerator _subject;

        public TokenResponseGeneratorTests()
        {
            _tokenService = new MockTokenService();
            _refreshTokenService = new MockRefreshTokenService();
            _scopeParser = new DefaultScopeParser();
            _resources = new MockResourceStore();
            _clients = new MockClientStore();
            _clock = new SystemClock();
            _logger = new MockLogger<TokenResponseGenerator>();

            _subject = new TokenResponseGenerator(
                _clock,
                _tokenService,
                _refreshTokenService,
                _scopeParser,
                _resources,
                _clients,
                _logger);
        }

        [Fact] 
        public async Task ProcessAsync_with_valid_client_credentials_should_return_token_response() 
        {
            // Arrange
            var request = new TokenRequestValidationResult
            {
                ValidatedRequest = new ValidatedTokenRequest
                {
                    Client = new Client 
                    { 
                        ClientId = "client1",
                        AllowedGrantTypes = new List<string> { OidcConstants.GrantTypes.ClientCredentials }
                    },
                    GrantType = OidcConstants.GrantTypes.ClientCredentials,
                    ValidatedResources = new ResourceValidationResult()
                },
                CustomResponse = new Dictionary<string, object>()
            };

            // Act
            var response = await _subject.ProcessAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrEmpty();
            response.AccessTokenLifetime.Should().Be(3600); // Default lifetime
        }

        [Fact]
        public async Task ProcessAsync_with_password_grant_should_return_token_response()
        {
            // Arrange
            var request = new TokenRequestValidationResult
            {
                ValidatedRequest = new ValidatedTokenRequest
                {
                    Client = new Client 
                    { 
                        ClientId = "client1",
                        AllowedGrantTypes = new List<string> { OidcConstants.GrantTypes.Password }
                    },
                    GrantType = OidcConstants.GrantTypes.Password,
                    ValidatedResources = new ResourceValidationResult(),
                    Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("sub", "123") }))
                },
                CustomResponse = new Dictionary<string, object>()
            };

            // Act
            var response = await _subject.ProcessAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrEmpty();
            response.AccessTokenLifetime.Should().Be(3600);
        }

        [Fact]
        public async Task ProcessAsync_with_refresh_token_grant_should_return_token_response()
        {
            // Arrange
            var request = new TokenRequestValidationResult
            {
                ValidatedRequest = new ValidatedTokenRequest
                {
                    Client = new Client 
                    { 
                        ClientId = "client1",
                        AllowedGrantTypes = new List<string> { OidcConstants.GrantTypes.RefreshToken },
                        AllowOfflineAccess = true
                    },
                    GrantType = OidcConstants.GrantTypes.RefreshToken,
                    ValidatedResources = new ResourceValidationResult(),
                    Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("sub", "123") }))
                },
                CustomResponse = new Dictionary<string, object>()
            };

            // Act
            var response = await _subject.ProcessAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ProcessAsync_with_custom_response_should_include_custom_parameters()
        {
            // Arrange
            var request = new TokenRequestValidationResult
            {
                ValidatedRequest = new ValidatedTokenRequest
                {
                    Client = new Client 
                    { 
                        ClientId = "client1",
                        AllowedGrantTypes = new List<string> { OidcConstants.GrantTypes.ClientCredentials }
                    },
                    GrantType = OidcConstants.GrantTypes.ClientCredentials,
                    ValidatedResources = new ResourceValidationResult()
                },
                CustomResponse = new Dictionary<string, object>
                {
                    { "custom_param", "custom_value" }
                }
            };

            // Act
            var response = await _subject.ProcessAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Custom.Should().NotBeNull();
            response.Custom.Should().ContainKey("custom_param");
            response.Custom["custom_param"].Should().Be("custom_value");
        }

        private class MockTokenService : ITokenService
        {
            public Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
            {
                return Task.FromResult(new Token { Claims = new List<Claim>() });
            }

            public Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
            {
                return Task.FromResult(new Token { Claims = new List<Claim>() });
            }

            public Task<string> CreateSecurityTokenAsync(Token token)
            {
                return Task.FromResult("test_token");
            }
        }

        private class MockRefreshTokenService : IRefreshTokenService
        {
            public Task<string> CreateRefreshTokenAsync(ClaimsPrincipal subject, Token accessToken, Client client)
            {
                return Task.FromResult("refresh_token");
            }

            public Task<string> UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken, Client client)
            {
                return Task.FromResult("updated_refresh_token");
            }
        }

        private class MockResourceStore : IResourceStore
        {
            public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
            {
                return Task.FromResult<IEnumerable<ApiResource>>(new List<ApiResource>());
            }

            public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
            {
                return Task.FromResult<IEnumerable<ApiResource>>(new List<ApiResource>());
            }

            public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
            {
                return Task.FromResult<IEnumerable<ApiScope>>(new List<ApiScope>());
            }

            public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
            {
                return Task.FromResult<IEnumerable<IdentityResource>>(new List<IdentityResource>());
            }

            public Task<Resources> GetAllResourcesAsync()
            {
                return Task.FromResult(new Resources());
            }
        }

        private class MockClientStore : IClientStore
        {
            public Task<Client> FindClientByIdAsync(string clientId)
            {
                return Task.FromResult(new Client { ClientId = clientId });
            }
        }

        private class MockLogger<T> : ILogger<T>
        {
            public IDisposable BeginScope<TState>(TState state) => null;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
        }
    }
}
