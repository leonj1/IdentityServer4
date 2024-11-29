using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class DefaultBackChannelLogoutServiceTests
    {
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IdentityServerTools> _mockTools;
        private readonly Mock<ILogoutNotificationService> _mockLogoutNotificationService;
        private readonly Mock<IBackChannelLogoutHttpClient> _mockHttpClient;
        private readonly Mock<ILogger<IBackChannelLogoutService>> _mockLogger;
        private readonly DefaultBackChannelLogoutService _service;

        public DefaultBackChannelLogoutServiceTests()
        {
            _mockClock = new Mock<ISystemClock>();
            _mockTools = new Mock<IdentityServerTools>();
            _mockLogoutNotificationService = new Mock<ILogoutNotificationService>();
            _mockHttpClient = new Mock<IBackChannelLogoutHttpClient>();
            _mockLogger = new Mock<ILogger<IBackChannelLogoutService>>();

            _service = new DefaultBackChannelLogoutService(
                _mockClock.Object,
                _mockTools.Object,
                _mockLogoutNotificationService.Object,
                _mockHttpClient.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task SendLogoutNotificationsAsync_WhenNoRequests_ShouldNotCallHttpClient()
        {
            // Arrange
            var context = new LogoutNotificationContext();
            _mockLogoutNotificationService
                .Setup(x => x.GetBackChannelLogoutNotificationsAsync(context))
                .ReturnsAsync(Enumerable.Empty<BackChannelLogoutRequest>());

            // Act
            await _service.SendLogoutNotificationsAsync(context);

            // Verify
            _mockHttpClient.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
        }

        [Fact]
        public async Task SendLogoutNotificationsAsync_WithValidRequest_ShouldSendNotification()
        {
            // Arrange
            var context = new LogoutNotificationContext();
            var request = new BackChannelLogoutRequest
            {
                ClientId = "client1",
                SubjectId = "subject1",
                SessionId = "session1",
                LogoutUri = "https://client1/logout"
            };

            _mockLogoutNotificationService
                .Setup(x => x.GetBackChannelLogoutNotificationsAsync(context))
                .ReturnsAsync(new[] { request });

            var expectedToken = "dummy_token";
            _mockTools
                .Setup(x => x.IssueJwtAsync(It.IsAny<int>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(expectedToken);

            // Act
            await _service.SendLogoutNotificationsAsync(context);

            // Verify
            _mockHttpClient.Verify(x => x.PostAsync(
                request.LogoutUri,
                It.Is<Dictionary<string, string>>(d => 
                    d.ContainsKey(OidcConstants.BackChannelLogoutRequest.LogoutToken) &&
                    d[OidcConstants.BackChannelLogoutRequest.LogoutToken] == expectedToken)
            ), Times.Once);
        }

        [Fact]
        public async Task CreateTokenAsync_WhenSessionIdRequired_AndMissing_ShouldThrowException()
        {
            // Arrange
            var request = new BackChannelLogoutRequest
            {
                ClientId = "client1",
                SubjectId = "subject1",
                SessionIdRequired = true,
                SessionId = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.SendLogoutNotificationsAsync(new LogoutNotificationContext { 
                    SubjectId = request.SubjectId,
                    SessionId = request.SessionId,
                    ClientIds = new[] { request.ClientId }
                })
            );
        }
    }
}
