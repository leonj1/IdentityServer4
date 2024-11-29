using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using PersistedGrantEntity = IdentityServer4.EntityFramework.Entities.PersistedGrant;

namespace IdentityServer4.EntityFramework.UnitTests.Stores
{
    public class PersistedGrantStoreTests : IDisposable
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly PersistedGrantStore _store;
        
        public PersistedGrantStoreTests()
        {
            var options = new DbContextOptionsBuilder<PersistedGrantDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PersistedGrantDbContext(options, new OperationalStoreOptions());
            _store = new PersistedGrantStore(
                _context,
                new Logger<PersistedGrantStore>(new LoggerFactory())
            );
        }

        [Fact]
        public async Task StoreAsync_WhenGrantDoesNotExist_ShouldCreateNew()
        {
            // Arrange
            var grant = new PersistedGrant
            {
                Key = "key1",
                Type = "type1",
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddHours(1),
                Data = "data1"
            };

            // Act
            await _store.StoreAsync(grant);

            // Assert
            var savedGrant = await _context.PersistedGrants.SingleOrDefaultAsync(x => x.Key == grant.Key);
            Assert.NotNull(savedGrant);
            Assert.Equal(grant.ClientId, savedGrant.ClientId);
            Assert.Equal(grant.Data, savedGrant.Data);
        }

        [Fact]
        public async Task GetAsync_WhenGrantExists_ShouldReturnGrant()
        {
            // Arrange
            var grant = new PersistedGrantEntity
            {
                Key = "key1",
                Type = "type1",
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddHours(1),
                Data = "data1"
            };
            _context.PersistedGrants.Add(grant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _store.GetAsync("key1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(grant.ClientId, result.ClientId);
            Assert.Equal(grant.Data, result.Data);
        }

        [Fact]
        public async Task RemoveAsync_WhenGrantExists_ShouldRemoveGrant()
        {
            // Arrange
            var grant = new PersistedGrantEntity
            {
                Key = "key1",
                Type = "type1",
                ClientId = "client1",
                SubjectId = "subject1",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddHours(1),
                Data = "data1"
            };
            _context.PersistedGrants.Add(grant);
            await _context.SaveChangesAsync();

            // Act
            await _store.RemoveAsync("key1");

            // Assert
            var result = await _context.PersistedGrants.SingleOrDefaultAsync(x => x.Key == "key1");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenGrantsExist_ShouldReturnFilteredGrants()
        {
            // Arrange
            var grants = new[]
            {
                new PersistedGrantEntity
                {
                    Key = "key1",
                    Type = "type1",
                    ClientId = "client1",
                    SubjectId = "subject1",
                    CreationTime = DateTime.UtcNow,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Data = "data1"
                },
                new PersistedGrantEntity
                {
                    Key = "key2",
                    Type = "type1",
                    ClientId = "client1",
                    SubjectId = "subject2",
                    CreationTime = DateTime.UtcNow,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Data = "data2"
                }
            };
            _context.PersistedGrants.AddRange(grants);
            await _context.SaveChangesAsync();

            // Act
            var filter = new PersistedGrantFilter
            {
                SubjectId = "subject1"
            };
            var result = await _store.GetAllAsync(filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("key1", result.First().Key);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
