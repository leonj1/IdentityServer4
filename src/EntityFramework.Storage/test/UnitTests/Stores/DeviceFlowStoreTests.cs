using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Security.Claims;

namespace IdentityServer4.EntityFramework.UnitTests.Stores
{
    public class DeviceFlowStoreTests : IDisposable
    {
        private readonly DbContextOptions<PersistedGrantDbContext> _options;
        private readonly PersistedGrantDbContext _context;
        private readonly ILogger<DeviceFlowStore> _logger;
        private readonly DeviceFlowStore _store;
        private readonly IPersistentGrantSerializer _serializer;

        public DeviceFlowStoreTests()
        {
            var name = Guid.NewGuid().ToString();
            _options = new DbContextOptionsBuilder<PersistedGrantDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            _context = new PersistedGrantDbContext(_options);
            _serializer = new PersistentGrantSerializer();
            _logger = new LoggerFactory().CreateLogger<DeviceFlowStore>();
            _store = new DeviceFlowStore(_context, _serializer, _logger);
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenSuccessful_ShouldCreateRecord()
        {
            // Arrange
            var deviceCode = "device_code";
            var userCode = "user_code";
            var data = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            // Act
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            // Assert
            var foundData = await _context.DeviceFlowCodes.FirstOrDefaultAsync();
            foundData.Should().NotBeNull();
            foundData.DeviceCode.Should().Be(deviceCode);
            foundData.UserCode.Should().Be(userCode);
            foundData.ClientId.Should().Be(data.ClientId);
        }

        [Fact]
        public async Task FindByUserCodeAsync_WhenCodeExists_ShouldReturnData()
        {
            // Arrange
            var deviceCode = "device_code";
            var userCode = "user_code";
            var data = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            // Act
            var foundData = await _store.FindByUserCodeAsync(userCode);

            // Assert
            foundData.Should().NotBeNull();
            foundData.ClientId.Should().Be(data.ClientId);
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_WhenCodeExists_ShouldReturnData()
        {
            // Arrange
            var deviceCode = "device_code";
            var userCode = "user_code";
            var data = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            // Act
            var foundData = await _store.FindByDeviceCodeAsync(deviceCode);

            // Assert
            foundData.Should().NotBeNull();
            foundData.ClientId.Should().Be(data.ClientId);
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_WhenCodeExists_ShouldUpdateData()
        {
            // Arrange
            var deviceCode = "device_code";
            var userCode = "user_code";
            var data = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            var updatedData = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(JwtClaimTypes.Subject, "123")
                }))
            };

            // Act
            await _store.UpdateByUserCodeAsync(userCode, updatedData);

            // Assert
            var foundData = await _context.DeviceFlowCodes.FirstOrDefaultAsync();
            foundData.Should().NotBeNull();
            foundData.SubjectId.Should().Be("123");
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenCodeExists_ShouldRemoveData()
        {
            // Arrange
            var deviceCode = "device_code";
            var userCode = "user_code";
            var data = new DeviceCode
            {
                ClientId = "client",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);

            // Act
            await _store.RemoveByDeviceCodeAsync(deviceCode);

            // Assert
            var foundData = await _context.DeviceFlowCodes.FirstOrDefaultAsync();
            foundData.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
