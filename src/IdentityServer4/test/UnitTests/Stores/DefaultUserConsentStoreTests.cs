using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests.Stores
{
    public class DefaultUserConsentStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _mockPersistedGrantStore;
        private readonly Mock<IPersistentGrantSerializer> _mockSerializer;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerationService;
        private readonly Mock<ILogger<DefaultUserConsentStore>> _mockLogger;
        private readonly DefaultUserConsentStore _store;

        public DefaultUserConsentStoreTests()
        {
            _mockPersistedGrantStore = new Mock<IPersistedGrantStore>();
            _mockSerializer = new Mock<IPersistentGrantSerializer>();
            _mockHandleGenerationService = new Mock<IHandleGenerationService>();
            _mockLogger = new Mock<ILogger<DefaultUserConsentStore>>();

            _store = new DefaultUserConsentStore(
                _mockPersistedGrantStore.Object,
                _mockSerializer.Object,
                _mockHandleGenerationService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task StoreUserConsentAsync_ShouldStoreConsent()
        {
            // Arrange
            var consent = new Consent
            {
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddDays(1)
            };

            var serializedConsent = "serialized_consent";
            _mockSerializer.Setup(x => x.Serialize(consent)).Returns(serializedConsent);

            // Act
            await _store.StoreUserConsentAsync(consent);

            // Assert
            _mockPersistedGrantStore.Verify(x => x.StoreAsync(It.Is<PersistedGrant>(g =>
                g.ClientId == consent.ClientId &&
                g.SubjectId == consent.SubjectId &&
                g.Type == IdentityServerConstants.PersistedGrantTypes.UserConsent &&
                g.Data == serializedConsent
            )), Times.Once);
        }

        [Fact]
        public async Task GetUserConsentAsync_ShouldRetrieveConsent()
        {
            // Arrange
            var subjectId = "subject1";
            var clientId = "client1";
            var key = $"{clientId}|{subjectId}";
            
            var persistedGrant = new PersistedGrant
            {
                Key = key,
                Type = IdentityServerConstants.PersistedGrantTypes.UserConsent,
                ClientId = clientId,
                SubjectId = subjectId,
                Data = "serialized_data"
            };

            var expectedConsent = new Consent
            {
                ClientId = clientId,
                SubjectId = subjectId
            };

            _mockPersistedGrantStore.Setup(x => x.GetAsync(key)).ReturnsAsync(persistedGrant);
            _mockSerializer.Setup(x => x.Deserialize<Consent>(persistedGrant.Data)).Returns(expectedConsent);

            // Act
            var result = await _store.GetUserConsentAsync(subjectId, clientId);

            // Assert
            result.Should().BeEquivalentTo(expectedConsent);
        }

        [Fact]
        public async Task RemoveUserConsentAsync_ShouldRemoveConsent()
        {
            // Arrange
            var subjectId = "subject1";
            var clientId = "client1";
            var key = $"{clientId}|{subjectId}";

            // Act
            await _store.RemoveUserConsentAsync(subjectId, clientId);

            // Assert
            _mockPersistedGrantStore.Verify(x => x.RemoveAsync(key), Times.Once);
        }
    }
}
