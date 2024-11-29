using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class IdentityServerToolsTests
    {
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<ITokenCreationService> _tokenCreation;
        private readonly Mock<ISystemClock> _clock;
        private readonly IdentityServerTools _subject;
        private readonly DateTime _now;

        public IdentityServerToolsTests()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _tokenCreation = new Mock<ITokenCreationService>();
            _clock = new Mock<ISystemClock>();
            
            _now = new DateTime(2020, 3, 10, 9, 0, 0, DateTimeKind.Utc);
            _clock.Setup(x => x.UtcNow).Returns(new DateTimeOffset(_now));

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("server");
            _contextAccessor.Setup(x => x.HttpContext).Returns(context);

            _subject = new IdentityServerTools(_contextAccessor.Object, _tokenCreation.Object, _clock.Object);
        }

        [Fact]
        public async Task IssueJwtAsync_with_default_issuer_should_create_token()
        {
            // Arrange
            var lifetime = 3600;
            var claims = new List<Claim> { new Claim("sub", "123") };
            var expectedToken = "token";

            _tokenCreation.Setup(x => x.CreateTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync(expectedToken);

            // Act
            var token = await _subject.IssueJwtAsync(lifetime, claims);

            // Assert
            token.Should().Be(expectedToken);
            _tokenCreation.Verify(x => x.CreateTokenAsync(It.Is<Token>(t =>
                t.Issuer == "https://server" &&
                t.Lifetime == lifetime &&
                t.CreationTime == _now
            )));
        }

        [Fact]
        public async Task IssueJwtAsync_with_custom_issuer_should_create_token()
        {
            // Arrange
            var lifetime = 3600;
            var issuer = "custom_issuer";
            var claims = new List<Claim> { new Claim("sub", "123") };
            var expectedToken = "token";

            _tokenCreation.Setup(x => x.CreateTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync(expectedToken);

            // Act
            var token = await _subject.IssueJwtAsync(lifetime, issuer, claims);

            // Assert
            token.Should().Be(expectedToken);
            _tokenCreation.Verify(x => x.CreateTokenAsync(It.Is<Token>(t =>
                t.Issuer == issuer &&
                t.Lifetime == lifetime &&
                t.CreationTime == _now
            )));
        }

        [Fact]
        public async Task IssueJwtAsync_with_null_claims_should_throw()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _subject.IssueJwtAsync(3600, (IEnumerable<Claim>)null));
        }

        [Fact]
        public async Task IssueJwtAsync_with_null_issuer_should_throw()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _subject.IssueJwtAsync(3600, null, new List<Claim>()));
        }
    }
}
