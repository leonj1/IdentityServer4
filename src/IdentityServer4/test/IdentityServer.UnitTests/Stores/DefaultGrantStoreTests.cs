using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Stores
{
    public class DefaultGrantStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _mockStore;
        private readonly Mock<IPersistentGrantSerializer> _mockSerializer;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerator;
        private readonly Mock<ILogger<DefaultGrantStore<TestGrantType>>> _mockLogger;
        private readonly DefaultGrantStore<TestGrantType> _subject;

        public DefaultGrantStoreTests()
        {
            _mockStore = new Mock<IPersistedGrantStore>();
            _mockSerializer = new Mock<IPersistentGrantSerializer>();
            _mockHandleGenerator = new Mock<IHandleGenerationService>();
            _mockLogger = new Mock<ILogger<DefaultGrantStore<TestGrantType>>>();

            _subject = new TestDefaultGrantStore(
                _mockStore.Object,
                _mockSerializer.Object,
                _mockHandleGenerator.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetItemAsync_WhenGrantExists_ShouldReturnDeserializedItem()
        {
            // Arrange
            var key = "test_key";
            var hashedKey = key.Sha256();
            var testData = "serialized_data";
            var expectedItem = new TestGrantType { Value = "test" };

            _mockStore.Setup(x => x.GetAsync(hashedKey))
                .ReturnsAsync(new PersistedGrant 
                { 
                    Key = hashedKey,
                    Type = "TestGrant",
                    Data = testData 
                });

            _mockSerializer.Setup(x => x.Deserialize<TestGrantType>(testData))
                .Returns(expectedItem);

            // Act
            var result = await _subject.GetItemPublic(key);

            // Assert
            result.Should().BeEquivalentTo(expectedItem);
            _mockStore.Verify(x => x.GetAsync(hashedKey), Times.Once);
            _mockSerializer.Verify(x => x.Deserialize<TestGrantType>(testData), Times.Once);
        }

        [Fact]
        public async Task CreateItemAsync_ShouldGenerateHandleAndStoreGrant()
        {
            // Arrange
            var item = new TestGrantType { Value = "test" };
            var handle = "generated_handle";
            var clientId = "client";
            var subjectId = "subject";
            var sessionId = "session";
            var description = "description";
            var created = DateTime.UtcNow;
            var lifetime = 3600;

            _mockHandleGenerator.Setup(x => x.GenerateAsync())
                .ReturnsAsync(handle);

            // Act
            var result = await _subject.CreateItemPublic(item, clientId, subjectId, sessionId, description, created, lifetime);

            // Assert
            result.Should().Be(handle);
            _mockHandleGenerator.Verify(x => x.GenerateAsync(), Times.Once);
            _mockStore.Verify(x => x.StoreAsync(It.Is<PersistedGrant>(g => 
                g.Key == handle.Sha256() && 
                g.ClientId == clientId &&
                g.SubjectId == subjectId &&
                g.SessionId == sessionId &&
                g.Description == description &&
                g.CreationTime == created &&
                g.Expiration == created.AddSeconds(lifetime)
            )), Times.Once);
        }

        private class TestGrantType
        {
            public string Value { get; set; }
        }

        private class TestDefaultGrantStore : DefaultGrantStore<TestGrantType>
        {
            public TestDefaultGrantStore(
                IPersistedGrantStore store,
                IPersistentGrantSerializer serializer,
                IHandleGenerationService handleGenerationService,
                ILogger logger)
                : base("TestGrant", store, serializer, handleGenerationService, logger)
            {
            }

            public Task<TestGrantType> GetItemPublic(string key)
            {
                return GetItemAsync(key);
            }

            public Task<string> CreateItemPublic(TestGrantType item, string clientId, string subjectId, string sessionId, string description, DateTime created, int lifetime)
            {
                return CreateItemAsync(item, clientId, subjectId, sessionId, description, created, lifetime);
            }
        }
    }
}
