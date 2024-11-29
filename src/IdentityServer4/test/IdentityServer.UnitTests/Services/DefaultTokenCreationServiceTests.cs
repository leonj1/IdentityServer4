using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultTokenCreationServiceTests
    {
        private DefaultTokenCreationService _sut;
        private MockKeyMaterialService _mockKeyService;
        private IdentityServerOptions _options;
        private MockSystemClock _mockClock;
        private ILogger<DefaultTokenCreationService> _logger;

        public DefaultTokenCreationServiceTests()
        {
            _mockKeyService = new MockKeyMaterialService();
            _options = new IdentityServerOptions();
            _mockClock = new MockSystemClock();
            _logger = new LoggerFactory().CreateLogger<DefaultTokenCreationService>();

            _sut = new DefaultTokenCreationService(
                _mockClock,
                _mockKeyService,
                _options,
                _logger);
        }

        [Fact]
        public async Task CreateTokenAsync_WhenValidTokenRequested_ShouldCreateValidJwt()
        {
            // Arrange
            var token = new Token
            {
                Audiences = new[] { "api1" },
                Issuer = "http://localhost",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600,
                Claims = new List<Claim>
                {
                    new Claim("sub", "123"),
                    new Claim("name", "Test User")
                },
                ClientId = "client1",
                AccessTokenType = AccessTokenType.Jwt,
                Type = TokenTypes.AccessToken
            };

            // Act
            var jwt = await _sut.CreateTokenAsync(token);

            // Assert
            jwt.Should().NotBeNullOrEmpty();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwt);
            
            jwtToken.Audiences.Should().Contain("api1");
            jwtToken.Issuer.Should().Be("http://localhost");
            jwtToken.Claims.Should().Contain(c => c.Type == "sub" && c.Value == "123");
            jwtToken.Claims.Should().Contain(c => c.Type == "name" && c.Value == "Test User");
        }

        [Fact]
        public async Task CreateTokenAsync_WhenNoSigningCredentials_ShouldThrowException()
        {
            // Arrange
            _mockKeyService.SigningCredentials = null;
            var token = new Token
            {
                Audiences = new[] { "api1" },
                Issuer = "http://localhost",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.CreateTokenAsync(token));
        }

        [Fact]
        public async Task CreateTokenAsync_WithCustomJwtType_ShouldIncludeTypeInHeader()
        {
            // Arrange
            _options.AccessTokenJwtType = "custom+jwt";
            var token = new Token
            {
                Audiences = new[] { "api1" },
                Issuer = "http://localhost",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600,
                Type = TokenTypes.AccessToken
            };

            // Act
            var jwt = await _sut.CreateTokenAsync(token);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwt);
            jwtToken.Header.Typ.Should().Be("custom+jwt");
        }
    }

    internal class MockSystemClock : ISystemClock
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;

        public DateTimeOffset UtcNow
        {
            get => _now;
            set => _now = value;
        }
    }
}
