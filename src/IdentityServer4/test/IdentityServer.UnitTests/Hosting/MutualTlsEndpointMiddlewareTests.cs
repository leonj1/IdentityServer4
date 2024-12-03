using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using NSubstitute;

namespace IdentityServer.UnitTests.Hosting
{
    public class MutualTlsEndpointMiddlewareTests
    {
        private readonly IdentityServerOptions _options;
        private readonly ILogger<MutualTlsEndpointMiddleware> _logger;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly HttpContext _context;
        private readonly RequestDelegate _next;

        public MutualTlsEndpointMiddlewareTests()
        {
            _options = new IdentityServerOptions();
            _logger = Substitute.For<ILogger<MutualTlsEndpointMiddleware>>();
            _schemes = Substitute.For<IAuthenticationSchemeProvider>();
            _context = new DefaultHttpContext();
            _next = Substitute.For<RequestDelegate>();
        }

        [Fact]
        public async Task When_MTLS_Not_Enabled_Should_Skip_Authentication()
        {
            // Arrange
            _options.MutualTls.Enabled = false;
            var middleware = new MutualTlsEndpointMiddleware(_next, _options, _logger);

            // Act
            await middleware.Invoke(_context, _schemes);

            // Assert
            await _next.Received(1).Invoke(_context);
            await _schemes.DidNotReceive().GetDefaultAuthenticateSchemeAsync();
        }

        [Fact]
        public async Task When_Domain_Based_MTLS_Should_Authenticate_On_Matching_Domain()
        {
            // Arrange
            _options.MutualTls.Enabled = true;
            _options.MutualTls.DomainName = "mtls.identityserver.com";
            _context.Request.Host = new HostString("mtls.identityserver.com");
            
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(new System.Security.Claims.ClaimsPrincipal(), "Certificate"));
            _context.AuthenticateAsync(Arg.Any<string>()).Returns(authResult);

            var middleware = new MutualTlsEndpointMiddleware(_next, _options, _logger);

            // Act
            await middleware.Invoke(_context, _schemes);

            // Assert
            await _next.Received(1).Invoke(_context);
        }

        [Fact]
        public async Task When_Path_Based_MTLS_Should_Rewrite_Path()
        {
            // Arrange
            _options.MutualTls.Enabled = true;
            _context.Request.Path = "/mtls/token";
            
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(new System.Security.Claims.ClaimsPrincipal(), "Certificate"));
            _context.AuthenticateAsync(Arg.Any<string>()).Returns(authResult);

            var middleware = new MutualTlsEndpointMiddleware(_next, _options, _logger);

            // Act
            await middleware.Invoke(_context, _schemes);

            // Assert
            _context.Request.Path.Should().Be("/connect/token");
            await _next.Received(1).Invoke(_context);
        }

        [Fact]
        public async Task When_Authentication_Fails_Should_Return_Forbidden()
        {
            // Arrange
            _options.MutualTls.Enabled = true;
            _options.MutualTls.DomainName = "mtls.identityserver.com";
            _context.Request.Host = new HostString("mtls.identityserver.com");
            
            var authResult = AuthenticateResult.Fail("Certificate validation failed");
            _context.AuthenticateAsync(Arg.Any<string>()).Returns(authResult);

            var middleware = new MutualTlsEndpointMiddleware(_next, _options, _logger);

            // Act
            await middleware.Invoke(_context, _schemes);

            // Assert
            await _next.DidNotReceive().Invoke(_context);
            _context.Response.StatusCode.Should().Be(403);
        }
    }
}
