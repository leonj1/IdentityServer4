using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Results
{
    public class TokenResultTests
    {
        private TokenResponse _tokenResponse;
        private TokenResult _target;
        private HttpContext _context;

        public TokenResultTests()
        {
            _tokenResponse = new TokenResponse
            {
                IdentityToken = "id_token",
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                AccessTokenLifetime = 3600,
                Scope = "openid profile",
                Custom = new Dictionary<string, object>
                {
                    { "custom_claim", "custom_value" }
                }
            };

            _target = new TokenResult(_tokenResponse);
            _context = new DefaultHttpContext();
        }

        [Fact]
        public void Constructor_WithNullResponse_ThrowsArgumentNullException()
        {
            Action act = () => new TokenResult(null);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("response");
        }

        [Fact]
        public async Task ExecuteAsync_ContainsAllExpectedValues()
        {
            // Act
            await _target.ExecuteAsync(_context);

            // Assert
            _context.Response.Headers["Cache-Control"].ToString().Should().Contain("no-store");
            _context.Response.Headers["Pragma"].ToString().Should().Contain("no-cache");
            
            var body = await ReadResponseBody();
            body.Should().Contain("\"id_token\":\"id_token\"");
            body.Should().Contain("\"access_token\":\"access_token\"");
            body.Should().Contain("\"refresh_token\":\"refresh_token\"");
            body.Should().Contain("\"expires_in\":3600");
            body.Should().Contain("\"token_type\":\"Bearer\"");
            body.Should().Contain("\"scope\":\"openid profile\"");
            body.Should().Contain("\"custom_claim\":\"custom_value\"");
        }

        private async Task<string> ReadResponseBody()
        {
            _context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            using var reader = new System.IO.StreamReader(_context.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}
