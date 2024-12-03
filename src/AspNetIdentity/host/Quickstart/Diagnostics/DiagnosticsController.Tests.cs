using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class DiagnosticsControllerTests
    {
        private readonly DiagnosticsController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<IAuthenticationService> _mockAuthService;

        public DiagnosticsControllerTests()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockAuthService = new Mock<IAuthenticationService>();

            // Setup authentication service
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);

            _mockHttpContext.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

            _controller = new DiagnosticsController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [Theory]
        [InlineData("127.0.0.1")]
        [InlineData("::1")]
        public async Task Index_LocalAddress_ReturnsViewResult(string ipAddress)
        {
            // Arrange
            var connection = new Mock<ConnectionInfo>();
            connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse(ipAddress));
            connection.Setup(c => c.LocalIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            _mockHttpContext.Setup(h => h.Connection).Returns(connection.Object);

            _mockAuthService
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), null))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity()), "Test")));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DiagnosticsViewModel>(viewResult.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task Index_RemoteAddress_ReturnsNotFound()
        {
            // Arrange
            var connection = new Mock<ConnectionInfo>();
            connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("192.168.1.1"));
            connection.Setup(c => c.LocalIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
            _mockHttpContext.Setup(h => h.Connection).Returns(connection.Object);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
