using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockLogoutNotificationServiceTests
    {
        private readonly MockLogoutNotificationService _sut;
        private readonly LogoutNotificationContext _context;

        public MockLogoutNotificationServiceTests()
        {
            _sut = new MockLogoutNotificationService();
            _context = new LogoutNotificationContext();
        }

        [Fact]
        public async Task GetFrontChannelLogoutNotificationsUrlsAsync_ShouldSetCalledFlag()
        {
            // Act
            await _sut.GetFrontChannelLogoutNotificationsUrlsAsync(_context);

            // Assert
            _sut.GetFrontChannelLogoutNotificationsUrlsCalled.Should().BeTrue();
        }

        [Fact]
        public async Task GetFrontChannelLogoutNotificationsUrlsAsync_ShouldReturnConfiguredUrls()
        {
            // Arrange
            var expectedUrls = new[] { "https://client1/signout", "https://client2/signout" };
            _sut.FrontChannelLogoutNotificationsUrls.AddRange(expectedUrls);

            // Act
            var result = await _sut.GetFrontChannelLogoutNotificationsUrlsAsync(_context);

            // Assert
            result.Should().BeEquivalentTo(expectedUrls);
        }

        [Fact]
        public async Task GetBackChannelLogoutNotificationsAsync_ShouldSetCalledFlag()
        {
            // Act
            await _sut.GetBackChannelLogoutNotificationsAsync(_context);

            // Assert
            _sut.SendBackChannelLogoutNotificationsCalled.Should().BeTrue();
        }

        [Fact]
        public async Task GetBackChannelLogoutNotificationsAsync_ShouldReturnConfiguredRequests()
        {
            // Arrange
            var requests = new[]
            {
                new BackChannelLogoutRequest { ClientId = "client1", LogoutUri = "https://client1/logout" },
                new BackChannelLogoutRequest { ClientId = "client2", LogoutUri = "https://client2/logout" }
            };
            _sut.BackChannelLogoutRequests.AddRange(requests);

            // Act
            var result = await _sut.GetBackChannelLogoutNotificationsAsync(_context);

            // Assert
            result.Should().BeEquivalentTo(requests);
        }
    }
}
