using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class DiagnosticsControllerTests
    {
        [Fact]
        public async Task Index_LocalAddress_ReturnsViewResult()
        {
            // Arrange
            var controller = new DiagnosticsController();
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            httpContext.Connection.LocalIpAddress = IPAddress.Parse("127.0.0.1");
            
            var authService = new Mock<IAuthenticationService>();
            authService
                .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity()), "Test")));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(authService.Object);
            
            httpContext.RequestServices = serviceProvider.Object;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DiagnosticsViewModel>(viewResult.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task Index_RemoteAddress_ReturnsNotFound()
        {
            // Arrange
            var controller = new DiagnosticsController();
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");
            httpContext.Connection.LocalIpAddress = IPAddress.Parse("127.0.0.1");
            
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
