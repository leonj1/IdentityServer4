using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Stores
{
    public class IPersistedGrantStoreTests
    {
        private readonly Mock<IPersistedGrantStore> _mockStore;
        private readonly PersistedGrant _testGrant;
        private readonly PersistedGrantFilter _testFilter;

        public IPersistedGrantStoreTests()
        {
            _mockStore = new Mock<IPersistedGrantStore>();
            
            _testGrant = new PersistedGrant
            {
                Key = "test-key",
                Type = "test-type",
                SubjectId = "test-subject",
                ClientId = "test-client",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddHours(1),
                Data = "test-data"
            };

            _testFilter = new PersistedGrantFilter
            {
                SubjectId = "test-subject",
                ClientId = "test-client",
                Type = "test-type"
            };
        }

        [Fact]
        public async Task StoreAsync_ShouldStoreGrant()
        {
            // Arrange
            _mockStore.Setup(x => x.StoreAsync(_testGrant))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.StoreAsync(_testGrant);

            // Assert
            _mockStore.Verify(x => x.StoreAsync(_testGrant), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldRetrieveGrant()
        {
            // Arrange
            _mockStore.Setup(x => x.GetAsync(_testGrant.Key))
                .ReturnsAsync(_testGrant);

            // Act
            var result = await _mockStore.Object.GetAsync(_testGrant.Key);

            // Assert
            Assert.Equal(_testGrant, result);
            _mockStore.Verify(x => x.GetAsync(_testGrant.Key), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldRetrieveAllGrants()
        {
            // Arrange
            var grants = new List<PersistedGrant> { _testGrant };
            _mockStore.Setup(x => x.GetAllAsync(_testFilter))
                .ReturnsAsync(grants);

            // Act
            var result = await _mockStore.Object.GetAllAsync(_testFilter);

            // Assert
            Assert.Equal(grants, result);
            _mockStore.Verify(x => x.GetAllAsync(_testFilter), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveGrant()
        {
            // Arrange
            _mockStore.Setup(x => x.RemoveAsync(_testGrant.Key))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.RemoveAsync(_testGrant.Key);

            // Assert
            _mockStore.Verify(x => x.RemoveAsync(_testGrant.Key), Times.Once);
        }

        [Fact]
        public async Task RemoveAllAsync_ShouldRemoveAllGrants()
        {
            // Arrange
            _mockStore.Setup(x => x.RemoveAllAsync(_testFilter))
                .Returns(Task.CompletedTask);

            // Act
            await _mockStore.Object.RemoveAllAsync(_testFilter);

            // Assert
            _mockStore.Verify(x => x.RemoveAllAsync(_testFilter), Times.Once);
        }
    }
}
