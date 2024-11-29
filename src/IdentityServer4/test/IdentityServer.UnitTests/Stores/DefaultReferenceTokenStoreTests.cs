using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Stores
{
    public class DefaultReferenceTokenStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _persistedGrantStore;
        private readonly Mock<IPersistentGrantSerializer> _serializer;
        private readonly Mock<IHandleGenerationService> _handleGenerationService;
        private readonly Mock<ILogger<DefaultReferenceTokenStore>> _logger;
        private readonly DefaultReferenceTokenStore _subject;

        public DefaultReferenceTokenStoreTests()
        {
            _persistedGrantStore = new Mock<IPersistedGrantStore>();
            _serializer = new Mock<IPersistentGrantSerializer>();
            _handleGenerationService = new Mock<IHandleGenerationService>();
            _logger = new Mock<ILogger<DefaultReferenceTokenStore>>();
            
            _subject = new DefaultReferenceTokenStore(
                _persistedGrantStore.Object,
                _serializer.Object,
                _handleGenerationService.Object,
                _logger.Object);
        }

        [Fact]
        public async Task StoreReferenceTokenAsync_WhenTokenIsValid_ShouldStoreToken()
        {
            // Arrange
            var token = new Token
            {
                ClientId = "client1",
                SubjectId = "subject1",
                SessionId = "session1",
                Description = "test token",
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600
            };
            
            var handle = "handle123";
            _handleGenerationService.Setup(x => x.GenerateAsync()).ReturnsAsync(handle);

            // Act
            var result = await _subject.StoreReferenceTokenAsync(token);

            // Assert
            result.Should().Be(handle);
            _persistedGrantStore.Verify(x => x.StoreAsync(It.Is<PersistedGrant>(g =>
                g.ClientId == token.ClientId &&
                g.SubjectId == token.SubjectId &&
                g.SessionId == token.SessionId &&
                g.Description == token.Description &&
                g.Type == IdentityServerConstants.PersistedGrantTypes.ReferenceToken
            )), Times.Once);
        }

        [Fact]
        public async Task GetReferenceTokenAsync_WhenTokenExists_ShouldReturnToken()
        {
            // Arrange
            var handle = "handle123";
            var persistedGrant = new PersistedGrant
            {
                Key = handle,
                Type = IdentityServerConstants.PersistedGrantTypes.ReferenceToken
            };
            var token = new Token();

            _persistedGrantStore.Setup(x => x.GetAsync(handle)).ReturnsAsync(persistedGrant);
            _serializer.Setup(x => x.Deserialize<Token>(persistedGrant.Data)).Returns(token);

            // Act
            var result = await _subject.GetReferenceTokenAsync(handle);

            // Assert
            result.Should().Be(token);
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_WhenTokenExists_ShouldRemoveToken()
        {
            // Arrange
            var handle = "handle123";

            // Act
            await _subject.RemoveReferenceTokenAsync(handle);

            // Assert
            _persistedGrantStore.Verify(x => x.RemoveAsync(handle), Times.Once);
        }

        [Fact]
        public async Task RemoveReferenceTokensAsync_ShouldRemoveAllTokensForSubjectAndClient()
        {
            // Arrange
            var subjectId = "subject1";
            var clientId = "client1";

            // Act
            await _subject.RemoveReferenceTokensAsync(subjectId, clientId);

            // Assert
            _persistedGrantStore.Verify(x => x.RemoveAllAsync(subjectId, clientId, 
                IdentityServerConstants.PersistedGrantTypes.ReferenceToken), Times.Once);
        }
    }
}
