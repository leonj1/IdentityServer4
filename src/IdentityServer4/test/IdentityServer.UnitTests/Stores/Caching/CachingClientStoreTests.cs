using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Stores.Caching
{
    public class CachingClientStoreTests
    {
        private readonly Mock<IClientStore> _inner;
        private readonly Mock<ICache<Client>> _cache;
        private readonly Mock<ILogger<CachingClientStore<IClientStore>>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly CachingClientStore<IClientStore> _subject;

        public CachingClientStoreTests()
        {
            _inner = new Mock<IClientStore>();
            _cache = new Mock<ICache<Client>>();
            _logger = new Mock<ILogger<CachingClientStore<IClientStore>>>();
            _options = new IdentityServerOptions();
            _subject = new CachingClientStore<IClientStore>(_options, _inner.Object, _cache.Object, _logger.Object);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ShouldReturnFromCache()
        {
            // Arrange
            var clientId = "test_client";
            var client = new Client { ClientId = clientId };
            
            _cache.Setup(x => x.GetAsync(clientId, _options.Caching.ClientStoreExpiration,
                    It.IsAny<Func<Task<Client>>>(), _logger.Object))
                .ReturnsAsync(client);

            // Act
            var result = await _subject.FindClientByIdAsync(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(client);
            _cache.Verify(x => x.GetAsync(clientId, _options.Caching.ClientStoreExpiration,
                It.IsAny<Func<Task<Client>>>(), _logger.Object), Times.Once);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var clientId = "nonexistent_client";
            
            _cache.Setup(x => x.GetAsync(clientId, _options.Caching.ClientStoreExpiration,
                    It.IsAny<Func<Task<Client>>>(), _logger.Object))
                .ReturnsAsync((Client)null);

            // Act
            var result = await _subject.FindClientByIdAsync(clientId);

            // Assert
            result.Should().BeNull();
            _cache.Verify(x => x.GetAsync(clientId, _options.Caching.ClientStoreExpiration,
                It.IsAny<Func<Task<Client>>>(), _logger.Object), Times.Once);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenCacheMisses_ShouldCheckInnerStore()
        {
            // Arrange
            var clientId = "test_client";
            var client = new Client { ClientId = clientId };
            
            _inner.Setup(x => x.FindClientByIdAsync(clientId))
                .ReturnsAsync(client);

            _cache.Setup(x => x.GetAsync(clientId, _options.Caching.ClientStoreExpiration,
                    It.IsAny<Func<Task<Client>>>(), _logger.Object))
                .Returns<string, TimeSpan?, Func<Task<Client>>, ILogger>((key, duration, getter, logger) => getter());

            // Act
            var result = await _subject.FindClientByIdAsync(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(client);
            _inner.Verify(x => x.FindClientByIdAsync(clientId), Times.Once);
        }
    }
}
