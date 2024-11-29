using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.EntityFramework.UnitTests
{
    public class TokenCleanupServiceTests
    {
        private readonly OperationalStoreOptions _options;
        private readonly Mock<IPersistedGrantDbContext> _persistedGrantDbContext;
        private readonly Mock<ILogger<TokenCleanupService>> _logger;
        private readonly Mock<IOperationalStoreNotification> _notification;
        private readonly TokenCleanupService _subject;

        public TokenCleanupServiceTests()
        {
            _options = new OperationalStoreOptions
            {
                TokenCleanupBatchSize = 100
            };

            _persistedGrantDbContext = new Mock<IPersistedGrantDbContext>();
            _logger = new Mock<ILogger<TokenCleanupService>>();
            _notification = new Mock<IOperationalStoreNotification>();

            _subject = new TokenCleanupService(
                _options,
                _persistedGrantDbContext.Object,
                _logger.Object,
                _notification.Object
            );
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredGrantsExist_ShouldRemoveThem()
        {
            // Arrange
            var expiredGrants = new[]
            {
                new PersistedGrant
                {
                    Key = "key1",
                    Expiration = DateTime.UtcNow.AddDays(-1)
                }
            };

            var grantsDbSet = MockDbSet(expiredGrants);
            _persistedGrantDbContext.Setup(x => x.PersistedGrants).Returns(grantsDbSet.Object);

            // Act
            await _subject.RemoveExpiredGrantsAsync();

            // Assert
            _persistedGrantDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            _notification.Verify(x => x.PersistedGrantsRemovedAsync(It.IsAny<IEnumerable<PersistedGrant>>()), Times.Once);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredDeviceCodesExist_ShouldRemoveThem()
        {
            // Arrange
            var expiredCodes = new[]
            {
                new DeviceFlowCodes
                {
                    DeviceCode = "code1",
                    Expiration = DateTime.UtcNow.AddDays(-1)
                }
            };

            var codesDbSet = MockDbSet(expiredCodes);
            _persistedGrantDbContext.Setup(x => x.DeviceFlowCodes).Returns(codesDbSet.Object);

            // Act
            await _subject.RemoveExpiredGrantsAsync();

            // Assert
            _persistedGrantDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            _notification.Verify(x => x.DeviceCodesRemovedAsync(It.IsAny<IEnumerable<DeviceFlowCodes>>()), Times.Once);
        }

        [Fact]
        public void Constructor_WhenOptionsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TokenCleanupService(
                null,
                _persistedGrantDbContext.Object,
                _logger.Object));
        }

        [Fact]
        public void Constructor_WhenBatchSizeLessThanOne_ThrowsArgumentException()
        {
            // Arrange
            var options = new OperationalStoreOptions
            {
                TokenCleanupBatchSize = 0
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new TokenCleanupService(
                options,
                _persistedGrantDbContext.Object,
                _logger.Object));
        }

        private static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> items) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            var queryable = items.AsQueryable();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }
    }
}
