using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer4.UnitTests.Validation
{
    public class JwtBearerClientAssertionSecretParserTests
    {
        private readonly IdentityServerOptions _options;
        private readonly JwtBearerClientAssertionSecretParser _parser;
        private readonly ILogger<JwtBearerClientAssertionSecretParser> _logger;

        public JwtBearerClientAssertionSecretParserTests()
        {
            _options = new IdentityServerOptions();
            _logger = new LoggerFactory().CreateLogger<JwtBearerClientAssertionSecretParser>();
            _parser = new JwtBearerClientAssertionSecretParser(_options, _logger);
        }

        [Fact]
        public void AuthenticationMethod_Should_Return_PrivateKeyJwt()
        {
            _parser.AuthenticationMethod.Should().Be(OidcConstants.EndpointAuthenticationMethods.PrivateKeyJwt);
        }

        [Fact]
        public async Task Parse_Should_Return_Null_When_ContentType_Not_Form()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";

            // Act
            var result = await _parser.ParseAsync(context);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Parse_Should_Return_Null_When_No_ClientAssertion()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());

            // Act
            var result = await _parser.ParseAsync(context);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Parse_Should_Return_ParsedSecret_When_Valid_JWT()
        {
            // Arrange
            var clientId = "client1";
            var jwt = CreateJwt(clientId);

            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            var form = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { OidcConstants.TokenRequest.ClientAssertionType, OidcConstants.ClientAssertionTypes.JwtBearer },
                { OidcConstants.TokenRequest.ClientAssertion, jwt }
            };
            context.Request.Form = new FormCollection(form);

            // Act
            var result = await _parser.ParseAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(clientId);
            result.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.JwtBearer);
            result.Credential.Should().Be(jwt);
        }

        private string CreateJwt(string clientId)
        {
            var securityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "issuer",
                audience: "audience",
                claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, clientId) },
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
