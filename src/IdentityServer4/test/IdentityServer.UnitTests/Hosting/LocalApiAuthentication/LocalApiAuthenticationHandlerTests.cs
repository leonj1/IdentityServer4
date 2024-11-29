using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Hosting.LocalApiAuthentication
{
    public class LocalApiAuthenticationHandlerTests
    {
        private readonly Mock<ITokenValidator> _tokenValidator;
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<IOptionsMonitor<LocalApiAuthenticationOptions>> _options;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly LocalApiAuthenticationOptions _authOptions;
        private readonly UrlEncoder _encoder;

        public LocalApiAuthenticationHandlerTests()
        {
            _tokenValidator = new Mock<ITokenValidator>();
            _clock = new Mock<ISystemClock>();
            _options = new Mock<IOptionsMonitor<LocalApiAuthenticationOptions>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _authOptions = new LocalApiAuthenticationOptions();
            _encoder = UrlEncoder.Default;

            _options.Setup(x => x.CurrentValue).Returns(_authOptions);
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger<LocalApiAuthenticationHandler>>());
        }

        [Fact]
        public async Task NoAuthorizationHeader_ShouldReturnNoResult()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var handler = CreateHandler(context);
            await handler.InitializeAsync(new AuthenticationScheme("Bearer", null, typeof(LocalApiAuthenticationHandler)), context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            result.None.Should().BeTrue();
        }

        [Fact]
        public async Task InvalidAuthorizationHeader_ShouldReturnFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Invalid");
            var handler = CreateHandler(context);
            await handler.InitializeAsync(new AuthenticationScheme("Bearer", null, typeof(LocalApiAuthenticationHandler)), context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Failure.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Bearer valid_token");
            
            var validationResult = new TokenValidationResult 
            { 
                IsError = false,
                Claims = new[] { new Claim("sub", "123") }
            };
            
            _tokenValidator.Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(validationResult);

            var handler = CreateHandler(context);
            await handler.InitializeAsync(new AuthenticationScheme("Bearer", null, typeof(LocalApiAuthenticationHandler)), context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Principal.Should().NotBeNull();
            result.Principal.Claims.Should().Contain(c => c.Type == "sub" && c.Value == "123");
        }

        [Fact]
        public async Task InvalidToken_ShouldReturnFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "Bearer invalid_token");
            
            var validationResult = new TokenValidationResult 
            { 
                IsError = true,
                Error = "Invalid token"
            };
            
            _tokenValidator.Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(validationResult);

            var handler = CreateHandler(context);
            await handler.InitializeAsync(new AuthenticationScheme("Bearer", null, typeof(LocalApiAuthenticationHandler)), context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Failure.Should().NotBeNull();
            result.Failure.Message.Should().Be("Invalid token");
        }

        private LocalApiAuthenticationHandler CreateHandler(HttpContext context)
        {
            return new LocalApiAuthenticationHandler(
                _options.Object,
                _loggerFactory.Object,
                _encoder,
                _clock.Object,
                _tokenValidator.Object);
        }
    }
}
