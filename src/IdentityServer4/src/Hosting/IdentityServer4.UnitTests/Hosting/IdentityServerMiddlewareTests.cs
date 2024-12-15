using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class IdentityServerMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<IdentityServerMiddleware>> _loggerMock;
        private readonly Mock<IEndpointRouter> _routerMock;
        private readonly Mock<IUserSession> _sessionMock;
        private readonly Mock<IEventService> _eventsMock;
        private readonly Mock<IBackChannelLogoutService> _backChannelLogoutServiceMock;
        private readonly HttpContext _httpContext;

        public IdentityServerMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<IdentityServerMiddleware>>();
            _routerMock = new Mock<IEndpointRouter>();
            _sessionMock = new Mock<IUserSession>();
            _eventsMock = new Mock<IEventService>();
            _backChannelLogoutServiceMock = new Mock<IBackChannelLogoutService>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task Invoke_WhenEndpointNotFound_ShouldCallNext()
        {
            // Arrange
            _routerMock.Setup(r => r.Find(It.IsAny<HttpContext>()))
                .Returns((Endpoint)null);

            var middleware = new IdentityServerMiddleware(
                _nextMock.Object,
                _loggerMock.Object);

            // Act
            await middleware.Invoke(
                _httpContext,
                _routerMock.Object,
                _sessionMock.Object,
                _eventsMock.Object,
                _backChannelLogoutServiceMock.Object);

            // Assert
            _nextMock.Verify(n => n(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_ShouldEnsureSessionIdCookie()
        {
            // Arrange
            var middleware = new IdentityServerMiddleware(
                _nextMock.Object,
                _loggerMock.Object);

            // Act
            await middleware.Invoke(
                _httpContext,
                _routerMock.Object,
                _sessionMock.Object,
                _eventsMock.Object,
                _backChannelLogoutServiceMock.Object);

            // Assert
            _sessionMock.Verify(s => s.EnsureSessionIdCookieAsync(), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenSignOutCalled_ShouldRemoveSessionIdCookie()
        {
            // Arrange
            var middleware = new IdentityServerMiddleware(
                _nextMock.Object,
                _loggerMock.Object);

            _httpContext.SetSignOutCalled();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await middleware.Invoke(
                    _httpContext,
                    _routerMock.Object,
                    _sessionMock.Object,
                    _eventsMock.Object,
                    _backChannelLogoutServiceMock.Object));

            _sessionMock.Verify(s => s.RemoveSessionIdCookieAsync(), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenExceptionOccurs_ShouldRaiseEvent()
        {
            // Arrange
            var expectedException = new Exception("Test exception");
            _routerMock.Setup(r => r.Find(It.IsAny<HttpContext>()))
                .Throws(expectedException);

            var middleware = new IdentityServerMiddleware(
                _nextMock.Object,
                _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await middleware.Invoke(
                    _httpContext,
                    _routerMock.Object,
                    _sessionMock.Object,
                    _eventsMock.Object,
                    _backChannelLogoutServiceMock.Object));

            _eventsMock.Verify(e => e.RaiseAsync(It.Is<UnhandledExceptionEvent>(
                evt => evt.Exception == expectedException)), Times.Once);
        }
    }
}
