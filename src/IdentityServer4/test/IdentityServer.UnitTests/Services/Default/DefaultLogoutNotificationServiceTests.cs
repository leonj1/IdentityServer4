using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultLogoutNotificationServiceTests
    {
        private readonly Mock<IClientStore> _clientStore;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ILogger<LogoutNotificationService>> _logger;
        private readonly LogoutNotificationService _service;
        private readonly HttpContext _httpContext;

        public DefaultLogoutNotificationServiceTests()
        {
            _clientStore = new Mock<IClientStore>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<LogoutNotificationService>>();
            
            _httpContext = new DefaultHttpContext();
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
            
            _service = new LogoutNotificationService(
                _clientStore.Object,
                _httpContextAccessor.Object,
                _logger.Object);
        }

        [Fact]
        public async Task GetFrontChannelLogoutNotificationsUrlsAsync_WhenNoClients_ReturnsEmptyList()
        {
            // Arrange
            var context = new LogoutNotificationContext();

            // Act
            var result = await _service.GetFrontChannelLogoutNotificationsUrlsAsync(context);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFrontChannelLogoutNotificationsUrlsAsync_WhenClientHasFrontChannelLogoutUri_ReturnsUrl()
        {
            // Arrange
            var clientId = "client1";
            var logoutUri = "https://client1/signout";
            var client = new Client
            {
                ClientId = clientId,
                FrontChannelLogoutUri = logoutUri,
                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect
            };

            _clientStore.Setup(x => x.FindEnabledClientByIdAsync(clientId))
                .ReturnsAsync(client);

            var context = new LogoutNotificationContext
            {
                ClientIds = new[] { clientId },
                SessionId = "session123"
            };

            // Act
            var result = await _service.GetFrontChannelLogoutNotificationsUrlsAsync(context);

            // Assert
            result.Should().ContainSingle();
            result.First().Should().Be(logoutUri);
        }

        [Fact]
        public async Task GetBackChannelLogoutNotificationsAsync_WhenClientHasBackChannelLogoutUri_ReturnsRequest()
        {
            // Arrange
            var clientId = "client1";
            var logoutUri = "https://client1/backchannel-signout";
            var client = new Client
            {
                ClientId = clientId,
                BackChannelLogoutUri = logoutUri,
                BackChannelLogoutSessionRequired = true
            };

            _clientStore.Setup(x => x.FindEnabledClientByIdAsync(clientId))
                .ReturnsAsync(client);

            var context = new LogoutNotificationContext
            {
                ClientIds = new[] { clientId },
                SessionId = "session123",
                SubjectId = "subject123"
            };

            // Act
            var result = await _service.GetBackChannelLogoutNotificationsAsync(context);

            // Assert
            result.Should().ContainSingle();
            var request = result.First();
            request.ClientId.Should().Be(clientId);
            request.LogoutUri.Should().Be(logoutUri);
            request.SessionId.Should().Be("session123");
            request.SubjectId.Should().Be("subject123");
            request.SessionIdRequired.Should().BeTrue();
        }

        [Fact]
        public async Task GetFrontChannelLogoutNotificationsUrlsAsync_WhenWsFederation_AddsCorrectParameters()
        {
            // Arrange
            var clientId = "client1";
            var logoutUri = "https://client1/signout";
            var client = new Client
            {
                ClientId = clientId,
                FrontChannelLogoutUri = logoutUri,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
            };

            _clientStore.Setup(x => x.FindEnabledClientByIdAsync(clientId))
                .ReturnsAsync(client);

            var context = new LogoutNotificationContext
            {
                ClientIds = new[] { clientId }
            };

            // Act
            var result = await _service.GetFrontChannelLogoutNotificationsUrlsAsync(context);

            // Assert
            result.Should().ContainSingle();
            result.First().Should().Contain("wa=wsignout1.0");
        }
    }
}
