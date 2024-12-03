using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using System.Security.Cryptography;

namespace IdentityServer4.UnitTests.Validation
{
    public class JwtRequestValidatorTests
    {
        private readonly ILogger<JwtRequestValidator> _logger;
        private readonly string _audience = "https://localhost";
        private readonly JwtRequestValidator _validator;
        private readonly RsaSecurityKey _rsaKey;
        private readonly Client _client;

        public JwtRequestValidatorTests()
        {
            _logger = new LoggerFactory().CreateLogger<JwtRequestValidator>();
            _validator = new JwtRequestValidator(_audience, _logger);
            
            // Generate RSA key for testing
            var rsa = RSA.Create();
            _rsaKey = new RsaSecurityKey(rsa);
            
            _client = new Client
            {
                ClientId = "client",
                ClientSecrets = new List<Secret>
                {
                    new Secret
                    {
                        Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                        Value = _rsaKey.ToString()
                    }
                }
            };
        }

        [Fact]
        public async Task Valid_JWT_Request_Should_Pass_Validation()
        {
            // Arrange
            var handler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
                new Claim("scope", "openid profile"),
                new Claim("response_type", "code")
            };

            var token = CreateToken(claims);
            var tokenString = handler.WriteToken(token);

            // Act
            var result = await _validator.ValidateAsync(_client, tokenString);

            // Assert
            result.IsError.Should().BeFalse();
            result.Payload.Should().ContainKey("scope");
            result.Payload["scope"].Should().Be("openid profile");
        }

        [Fact]
        public async Task Invalid_Audience_Should_Fail()
        {
            // Arrange
            var handler = new JwtSecurityTokenHandler();
            var claims = new[] { new Claim("scope", "openid") };
            
            var token = CreateToken(claims, "wrong_audience");
            var tokenString = handler.WriteToken(token);

            // Act
            var result = await _validator.ValidateAsync(_client, tokenString);

            // Assert
            result.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task Expired_Token_Should_Fail()
        {
            // Arrange
            var handler = new JwtSecurityTokenHandler();
            var claims = new[] { new Claim("scope", "openid") };
            
            var token = CreateToken(claims, _audience, DateTime.UtcNow.AddHours(-1));
            var tokenString = handler.WriteToken(token);

            // Act
            var result = await _validator.ValidateAsync(_client, tokenString);

            // Assert
            result.IsError.Should().BeTrue();
        }

        private JwtSecurityToken CreateToken(
            IEnumerable<Claim> claims,
            string audience = null,
            DateTime? expires = null)
        {
            var signingCredentials = new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256);
            
            return new JwtSecurityToken(
                issuer: _client.ClientId,
                audience: audience ?? _audience,
                claims: claims,
                expires: expires ?? DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signingCredentials,
                header: new JwtHeader(signingCredentials) 
                {
                    { JwtHeaderParameterNames.Typ, JwtClaimTypes.JwtTypes.AuthorizationRequest }
                }
            );
        }
    }
}
