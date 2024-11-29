using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.Entities;
using Xunit;
using FluentAssertions;

namespace IdentityServer4.EntityFramework.UnitTests.TokenCleanup
{
    public class IOperationalStoreNotificationTests
    {
        private class TestOperationalStoreNotification : IOperationalStoreNotification
        {
            public List<PersistedGrant> RemovedGrants { get; } = new List<PersistedGrant>();
            public List<DeviceFlowCodes> RemovedDeviceCodes { get; } = new List<DeviceFlowCodes>();

            public Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants)
            {
                RemovedGrants.AddRange(persistedGrants);
                return Task.CompletedTask;
            }

            public Task DeviceCodesRemovedAsync(IEnumerable<DeviceFlowCodes> deviceCodes)
            {
                RemovedDeviceCodes.AddRange(deviceCodes);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task PersistedGrantsRemovedAsync_ShouldReceiveNotification()
        {
            // Arrange
            var notification = new TestOperationalStoreNotification();
            var grants = new List<PersistedGrant>
            {
                new PersistedGrant { Key = "key1" },
                new PersistedGrant { Key = "key2" }
            };

            // Act
            await notification.PersistedGrantsRemovedAsync(grants);

            // Assert
            notification.RemovedGrants.Should().HaveCount(2);
            notification.RemovedGrants.Should().Contain(g => g.Key == "key1");
            notification.RemovedGrants.Should().Contain(g => g.Key == "key2");
        }

        [Fact]
        public async Task DeviceCodesRemovedAsync_ShouldReceiveNotification()
        {
            // Arrange
            var notification = new TestOperationalStoreNotification();
            var deviceCodes = new List<DeviceFlowCodes>
            {
                new DeviceFlowCodes { DeviceCode = "code1" },
                new DeviceFlowCodes { DeviceCode = "code2" }
            };

            // Act
            await notification.DeviceCodesRemovedAsync(deviceCodes);

            // Assert
            notification.RemovedDeviceCodes.Should().HaveCount(2);
            notification.RemovedDeviceCodes.Should().Contain(d => d.DeviceCode == "code1");
            notification.RemovedDeviceCodes.Should().Contain(d => d.DeviceCode == "code2");
        }
    }
}
