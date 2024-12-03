using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Extensions
{
    public class ProfileDataRequestContextExtensionsTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Client _client;
        private readonly ClaimsPrincipal _subject;

        public ProfileDataRequestContextExtensionsTests()
        {
            _logger = new Mock<ILogger>();
            _client = new Client { ClientId = "test_client", ClientName = "Test Client" };
            _subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));
        }

        [Fact]
        public void FilterClaims_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ProfileDataRequestContext context = null;
            var claims = new List<Claim>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.FilterClaims(claims));
        }

        [Fact]
        public void FilterClaims_WhenClaimsIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var context = new ProfileDataRequestContext(_subject, _client, "caller", new string[] { "claim1" });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.FilterClaims(null));
        }

        [Fact]
        public void FilterClaims_ShouldReturnOnlyRequestedClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("claim1", "value1"),
                new Claim("claim2", "value2"),
                new Claim("claim3", "value3")
            };

            var context = new ProfileDataRequestContext(
                _subject,
                _client,
                "caller",
                new string[] { "claim1", "claim3" }
            );

            // Act
            var result = context.FilterClaims(claims);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Type == "claim1");
            result.Should().Contain(x => x.Type == "claim3");
            result.Should().NotContain(x => x.Type == "claim2");
        }

        [Fact]
        public void AddRequestedClaims_WhenNoRequestedClaimTypes_ShouldNotAddClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("claim1", "value1"),
                new Claim("claim2", "value2")
            };

            var context = new ProfileDataRequestContext(
                _subject,
                _client,
                "caller",
                new string[] { }
            );

            // Act
            context.AddRequestedClaims(claims);

            // Assert
            context.IssuedClaims.Should().BeEmpty();
        }

        [Fact]
        public void AddRequestedClaims_ShouldAddOnlyRequestedClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("claim1", "value1"),
                new Claim("claim2", "value2"),
                new Claim("claim3", "value3")
            };

            var context = new ProfileDataRequestContext(
                _subject,
                _client,
                "caller",
                new string[] { "claim1", "claim3" }
            );

            // Act
            context.AddRequestedClaims(claims);

            // Assert
            context.IssuedClaims.Should().HaveCount(2);
            context.IssuedClaims.Should().Contain(x => x.Type == "claim1");
            context.IssuedClaims.Should().Contain(x => x.Type == "claim3");
            context.IssuedClaims.Should().NotContain(x => x.Type == "claim2");
        }

        [Fact]
        public void LogProfileRequest_ShouldLogCorrectly()
        {
            // Arrange
            var context = new ProfileDataRequestContext(
                _subject,
                _client,
                "caller",
                new string[] { "claim1", "claim2" }
            );

            // Act
            context.LogProfileRequest(_logger.Object);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void LogIssuedClaims_ShouldLogCorrectly()
        {
            // Arrange
            var context = new ProfileDataRequestContext(
                _subject,
                _client,
                "caller",
                new string[] { "claim1" }
            );
            context.IssuedClaims.Add(new Claim("claim1", "value1"));

            // Act
            context.LogIssuedClaims(_logger.Object);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
