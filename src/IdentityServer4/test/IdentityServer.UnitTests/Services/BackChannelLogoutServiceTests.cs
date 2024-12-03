using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class BackChannelLogoutServiceTests
    {
        private class TestBackChannelLogoutService : IBackChannelLogoutService
        {
            public bool WasCalled { get; private set; }
            public LogoutNotificationContext ReceivedContext { get; private set; }

            public Task SendLogoutNotificationsAsync(LogoutNotificationContext context)
            {
                WasCalled = true;
                ReceivedContext = context;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task SendLogoutNotificationsAsync_WhenCalled_ShouldReceiveContext()
        {
            // Arrange
            var service = new TestBackChannelLogoutService();
            var context = new LogoutNotificationContext();

            // Act
            await service.SendLogoutNotificationsAsync(context);

            // Assert
            service.WasCalled.Should().BeTrue();
            service.ReceivedContext.Should().BeSameAs(context);
        }

        [Fact]
        public async Task SendLogoutNotificationsAsync_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var service = new TestBackChannelLogoutService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await service.SendLogoutNotificationsAsync(null)
            );
        }
    }
}
