using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Linq;

namespace IdentityServer.UnitTests.Extensions
{
    public class IdentityServerToolsExtensionsTests
    {
        private readonly IdentityServerTools _tools;
        private readonly HttpContext _httpContext;
        private readonly IServiceProvider _serviceProvider;

        public IdentityServerToolsExtensionsTests()
        {
            _httpContext = new DefaultHttpContext();
            
            var services = new ServiceCollection();
            services.AddSingleton(new IdentityServerOptions { 
                EmitStaticAudienceClaim = true,
                IssuerUri = "https://identityserver"
            });
            
            _serviceProvider = services.BuildServiceProvider();
            _httpContext.RequestServices = _serviceProvider;
            
            var contextAccessor = new HttpContextAccessor
            {
                HttpContext = _httpContext
            };
            
            _tools = new IdentityServerTools(contextAccessor);
        }

        [Fact]
        public async Task IssueClientJwtAsync_ShouldIncludeClientIdClaim()
        {
            // Arrange
            var clientId = "test_client";
            var lifetime = 3600;

            // Act
            var jwt = await _tools.IssueClientJwtAsync(clientId, lifetime);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            token.Claims.Should().Contain(c => 
                c.Type == JwtClaimTypes.ClientId && 
                c.Value == clientId);
        }

        [Fact]
        public async Task IssueClientJwtAsync_WithScopes_ShouldIncludeScopeClaims()
        {
            // Arrange
            var clientId = "test_client";
            var lifetime = 3600;
            var scopes = new[] { "api1", "api2" };

            // Act
            var jwt = await _tools.IssueClientJwtAsync(clientId, lifetime, scopes);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            token.Claims.Where(c => c.Type == JwtClaimTypes.Scope)
                .Select(c => c.Value)
                .Should()
                .BeEquivalentTo(scopes);
        }

        [Fact]
        public async Task IssueClientJwtAsync_WithAudiences_ShouldIncludeAudienceClaims()
        {
            // Arrange
            var clientId = "test_client";
            var lifetime = 3600;
            var audiences = new[] { "aud1", "aud2" };

            // Act
            var jwt = await _tools.IssueClientJwtAsync(clientId, lifetime, audiences: audiences);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            token.Claims.Where(c => c.Type == JwtClaimTypes.Audience)
                .Select(c => c.Value)
                .Should()
                .Contain(audiences);
        }

        [Fact]
        public async Task IssueClientJwtAsync_WithAdditionalClaims_ShouldIncludeAllClaims()
        {
            // Arrange
            var clientId = "test_client";
            var lifetime = 3600;
            var additionalClaims = new[]
            {
                new Claim("custom1", "value1"),
                new Claim("custom2", "value2")
            };

            // Act
            var jwt = await _tools.IssueClientJwtAsync(clientId, lifetime, additionalClaims: additionalClaims);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            foreach (var claim in additionalClaims)
            {
                token.Claims.Should().Contain(c => 
                    c.Type == claim.Type && 
                    c.Value == claim.Value);
            }
        }
    }
}
