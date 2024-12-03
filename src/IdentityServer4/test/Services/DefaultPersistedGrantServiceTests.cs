using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Services
{
    public class DefaultPersistedGrantServiceTests
    {
        private readonly Mock<IPersistedGrantStore> _store;
        private readonly Mock<IPersistentGrantSerializer> _serializer;
        private readonly Mock<ILogger<DefaultPersistedGrantService>> _logger;
        private readonly DefaultPersistedGrantService _service;

        public DefaultPersistedGrantServiceTests()
        {
            _store = new Mock<IPersistedGrantStore>();
            _serializer = new Mock<IPersistentGrantSerializer>();
            _logger = new Mock<ILogger<DefaultPersistedGrantService>>();
            _service = new DefaultPersistedGrantService(_store.Object, _serializer.Object, _logger.Object);
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenNoGrants_ShouldReturnEmptyList()
        {
            // Arrange
            _store.Setup(x => x.GetAllAsync(It.IsAny<PersistedGrantFilter>()))
                  .ReturnsAsync(new List<PersistedGrant>());

            // Act
            var result = await _service.GetAllGrantsAsync("subject123");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenUserConsentsExist_ShouldReturnValidGrants()
        {
            // Arrange
            var consent = new Consent
            {
                ClientId = "client123",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddDays(1),
                Scopes = new[] { "scope1", "scope2" }
            };

            var grants = new List<PersistedGrant>
            {
                new PersistedGrant
                {
                    Type = IdentityServerConstants.PersistedGrantTypes.UserConsent,
                    ClientId = "client123",
                    SubjectId = "subject123",
                    Data = "serialized_data"
                }
            };

            _store.Setup(x => x.GetAllAsync(It.IsAny<PersistedGrantFilter>()))
                  .ReturnsAsync(grants);
            _serializer.Setup(x => x.Deserialize<Consent>(It.IsAny<string>()))
                      .Returns(consent);

            // Act
            var result = await _service.GetAllGrantsAsync("subject123");

            // Assert
            result.Should().HaveCount(1);
            var grant = result.First();
            grant.ClientId.Should().Be("client123");
            grant.Scopes.Should().BeEquivalentTo(new[] { "scope1", "scope2" });
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_ShouldCallStore()
        {
            // Arrange
            var subjectId = "subject123";
            var clientId = "client123";
            var sessionId = "session123";

            // Act
            await _service.RemoveAllGrantsAsync(subjectId, clientId, sessionId);

            // Assert
            _store.Verify(x => x.RemoveAllAsync(It.Is<PersistedGrantFilter>(f =>
                f.SubjectId == subjectId &&
                f.ClientId == clientId &&
                f.SessionId == sessionId)), Times.Once);
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenNullSubjectId_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _service.GetAllGrantsAsync(null));
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_WhenNullSubjectId_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _service.RemoveAllGrantsAsync(null));
        }

        [Fact]
        public async Task GetAllGrantsAsync_WhenMultipleGrantTypes_ShouldAggregateScopes()
        {
            // Arrange
            var grants = new List<PersistedGrant>
            {
                new PersistedGrant
                {
                    Type = IdentityServerConstants.PersistedGrantTypes.UserConsent,
                    ClientId = "client123",
                    SubjectId = "subject123",
                    Data = "consent_data"
                },
                new PersistedGrant
                {
                    Type = IdentityServerConstants.PersistedGrantTypes.RefreshToken,
                    ClientId = "client123",
                    SubjectId = "subject123",
                    Data = "refresh_token_data"
                }
            };

            var consent = new Consent
            {
                ClientId = "client123",
                Scopes = new[] { "scope1", "scope2" }
            };

            var refreshToken = new RefreshToken
            {
                AccessToken = new Token
                {
                    Claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim("scope", "scope3")
                    }
                }
            };

            _store.Setup(x => x.GetAllAsync(It.IsAny<PersistedGrantFilter>()))
                  .ReturnsAsync(grants);
            _serializer.Setup(x => x.Deserialize<Consent>("consent_data"))
                      .Returns(consent);
            _serializer.Setup(x => x.Deserialize<RefreshToken>("refresh_token_data"))
                      .Returns(refreshToken);

            // Act
            var result = await _service.GetAllGrantsAsync("subject123");

            // Assert
            result.Should().HaveCount(1);
            var grant = result.First();
            grant.Scopes.Should().BeEquivalentTo(new[] { "scope1", "scope2", "scope3" });
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_WithNullSessionId_ShouldNotFilterBySession()
        {
            // Arrange
            var subjectId = "subject123";
            var clientId = "client123";

            // Act
            await _service.RemoveAllGrantsAsync(subjectId, clientId, null);

            // Assert
            _store.Verify(x => x.RemoveAllAsync(It.Is<PersistedGrantFilter>(f =>
                f.SubjectId == subjectId &&
                f.ClientId == clientId &&
                f.SessionId == null)), Times.Once);
        }

        [Fact]
        public async Task RemoveAllGrantsAsync_WithClientIdOnly_ShouldRemoveClientGrants()
        {
            // Arrange
            var subjectId = "subject123";
            var clientId = "client123";

            // Act
            await _service.RemoveAllGrantsAsync(subjectId, clientId);

            // Assert
            _store.Verify(x => x.RemoveAllAsync(It.Is<PersistedGrantFilter>(f =>
                f.SubjectId == subjectId &&
                f.ClientId == clientId &&
                f.SessionId == null)), Times.Once);
        }
    }
}
