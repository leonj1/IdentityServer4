using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores.Default;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Stores
{
    public class DistributedCacheAuthorizationParametersMessageStoreTests
    {
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerator;
        private readonly DistributedCacheAuthorizationParametersMessageStore _store;

        public DistributedCacheAuthorizationParametersMessageStoreTests()
        {
            _mockCache = new Mock<IDistributedCache>();
            _mockHandleGenerator = new Mock<IHandleGenerationService>();
            _store = new DistributedCacheAuthorizationParametersMessageStore(_mockCache.Object, _mockHandleGenerator.Object);
        }

        [Fact]
        public async Task WriteAsync_ShouldGenerateKeyAndStoreMessage()
        {
            // Arrange
            var message = new Message<IDictionary<string, string[]>>(
                new Dictionary<string, string[]> 
                { 
                    { "test", new[] { "value" } },
                    { OidcConstants.AuthorizeRequest.RequestUri, new[] { "should_be_removed" } }
                });
            var expectedKey = "test_key";
            
            _mockHandleGenerator.Setup(x => x.GenerateAsync())
                .ReturnsAsync(expectedKey);

            // Act
            var result = await _store.WriteAsync(message);

            // Assert
            result.Should().Be(expectedKey);
            _mockCache.Verify(x => x.SetStringAsync(
                It.Is<string>(s => s.EndsWith(expectedKey)),
                It.IsAny<string>(),
                It.IsAny<DistributedCacheEntryOptions>()),
                Times.Once);
            message.Data.Should().NotContainKey(OidcConstants.AuthorizeRequest.RequestUri);
        }

        [Fact]
        public async Task ReadAsync_WhenKeyExists_ShouldReturnMessage()
        {
            // Arrange
            var id = "test_key";
            var messageData = new Dictionary<string, string[]> { { "test", new[] { "value" } } };
            var message = new Message<IDictionary<string, string[]>>(messageData);
            var serializedMessage = ObjectSerializer.ToString(message);

            _mockCache.Setup(x => x.GetStringAsync($"DistributedCacheAuthorizationParametersMessageStore-{id}"))
                .ReturnsAsync(serializedMessage);

            // Act
            var result = await _store.ReadAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().ContainKey("test");
            result.Data["test"].Should().BeEquivalentTo(new[] { "value" });
        }

        [Fact]
        public async Task ReadAsync_WhenKeyDoesNotExist_ShouldReturnEmptyMessage()
        {
            // Arrange
            var id = "nonexistent_key";
            _mockCache.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _store.ReadAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRemoveAsync()
        {
            // Arrange
            var id = "test_key";

            // Act
            await _store.DeleteAsync(id);

            // Assert
            _mockCache.Verify(x => x.RemoveAsync(id, default), Times.Once);
        }
    }
}
