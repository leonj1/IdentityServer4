using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Validation
{
    public class PrivateKeyJwtSecretValidatorTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Mock<IReplayCache> _replayCache;
        private readonly Mock<ILogger<PrivateKeyJwtSecretValidator>> _logger;
        private readonly PrivateKeyJwtSecretValidator _validator;
        private readonly RsaSecurityKey _rsaKey;
        private readonly string _clientId = "client";

        public PrivateKeyJwtSecretValidatorTests()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("server");
            
            _httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = httpContext
            };

            _replayCache = new Mock<IReplayCache>();
            _logger = new Mock<ILogger<PrivateKeyJwtSecretValidator>>();
            
            _validator = new PrivateKeyJwtSecretValidator(
                _httpContextAccessor,
                _replayCache.Object,
                _logger.Object);

            // Generate RSA key for testing
            var rsa = RSA.Create();
            _rsaKey = new RsaSecurityKey(rsa);
        }

        [Fact]
        public async Task Valid_Jwt_Token_Should_Validate_Successfully()
        {
            // Arrange
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                    Value = _rsaKey.ToString()
                }
            };

            var token = CreateToken();
            var parsedSecret = new ParsedSecret
            {
                Id = _clientId,
                Credential = token,
                Type = IdentityServerConstants.ParsedSecretTypes.JwtBearer
            };

            _replayCache.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Invalid_Secret_Type_Should_Fail()
        {
            // Arrange
            var secrets = new List<Secret>
            {
                new Secret { Type = "invalid" }
            };

            var parsedSecret = new ParsedSecret
            {
                Id = _clientId,
                Credential = "invalid",
                Type = "invalid"
            };

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Replayed_Token_Should_Fail()
        {
            // Arrange
            var secrets = new List<Secret>
            {
                new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                    Value = _rsaKey.ToString()
                }
            };

            var token = CreateToken();
            var parsedSecret = new ParsedSecret
            {
                Id = _clientId,
                Credential = token,
                Type = IdentityServerConstants.ParsedSecretTypes.JwtBearer
            };

            _replayCache.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _validator.ValidateAsync(secrets, parsedSecret);

            // Assert
            result.Success.Should().BeFalse();
        }

        private string CreateToken()
        {
            var now = DateTime.UtcNow;
            var handler = new JwtSecurityTokenHandler();
            
            var jwt = new JwtSecurityToken(
                issuer: _clientId,
                audience: $"https://server/connect/token",
                claims: new[] {
                    new Claim("sub", _clientId),
                    new Claim("jti", Guid.NewGuid().ToString())
                },
                notBefore: now,
                expires: now.AddMinutes(1),
                signingCredentials: new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256)
            );

            return handler.WriteToken(jwt);
        }
    }
}
