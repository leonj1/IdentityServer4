using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class IdentityServerMiddlewareTests
    {
        private readonly Mock<ILogger<IdentityServerMiddleware>> _logger;
        private readonly Mock<IEndpointRouter> _router;
        private readonly Mock<IUserSession> _session;
        private readonly Mock<IEventService> _events;
        private readonly Mock<IBackChannelLogoutService> _backChannelLogoutService;
        private readonly HttpContext _context;

        public IdentityServerMiddlewareTests()
        {
            _logger = new Mock<ILogger<IdentityServerMiddleware>>();
            _router = new Mock<IEndpointRouter>();
            _session = new Mock<IUserSession>();
            _events = new Mock<IEventService>();
            _backChannelLogoutService = new Mock<IBackChannelLogoutService>();
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task Invoke_WhenEndpointFound_ShouldProcessEndpoint()
        {
            // Arrange
            var mockEndpoint = new Mock<IEndpointHandler>();
            mockEndpoint.Setup(x => x.ProcessAsync(It.IsAny<HttpContext>()))
                       .ReturnsAsync((IEndpointResult)null);

            _router.Setup(x => x.Find(It.IsAny<HttpContext>()))
                  .Returns(mockEndpoint.Object);

            var middleware = new IdentityServerMiddleware(
                next: (ctx) => Task.CompletedTask,
                logger: _logger.Object);

            // Act
            await middleware.Invoke(
                _context,
                _router.Object,
                _session.Object,
                _events.Object,
                _backChannelLogoutService.Object);

            // Assert
            mockEndpoint.Verify(x => x.ProcessAsync(It.IsAny<HttpContext>()), Times.Once);
            _session.Verify(x => x.EnsureSessionIdCookieAsync(), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenSignOutCalled_ShouldProcessLogout()
        {
            // Arrange
            var logoutContext = new LogoutNotificationContext();
            _session.Setup(x => x.GetLogoutNotificationContext())
                   .ReturnsAsync(logoutContext);

            var middleware = new IdentityServerMiddleware(
                next: (ctx) => Task.CompletedTask,
                logger: _logger.Object);

            _context.SetSignOutCalled();

            // Act
            await middleware.Invoke(
                _context,
                _router.Object,
                _session.Object,
                _events.Object,
                _backChannelLogoutService.Object);

            // Assert
            _session.Verify(x => x.RemoveSessionIdCookieAsync(), Times.Once);
            _backChannelLogoutService.Verify(
                x => x.SendLogoutNotificationsAsync(It.IsAny<LogoutNotificationContext>()),
                Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenExceptionThrown_ShouldRaiseEvent()
        {
            // Arrange
            var expectedException = new Exception("Test exception");
            _router.Setup(x => x.Find(It.IsAny<HttpContext>()))
                  .Throws(expectedException);

            var middleware = new IdentityServerMiddleware(
                next: (ctx) => Task.CompletedTask,
                logger: _logger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await middleware.Invoke(
                    _context,
                    _router.Object,
                    _session.Object,
                    _events.Object,
                    _backChannelLogoutService.Object));

            _events.Verify(x => x.RaiseAsync(It.IsAny<UnhandledExceptionEvent>()), Times.Once);
        }
    }
}
