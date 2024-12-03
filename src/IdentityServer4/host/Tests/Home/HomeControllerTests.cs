using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServerHost.Tests.Home
{
    public class HomeControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _interactionMock;
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _interactionMock = new Mock<IIdentityServerInteractionService>();
            _environmentMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            
            _controller = new HomeController(
                _interactionMock.Object,
                _environmentMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Index_WhenInDevelopment_ReturnsViewResult()
        {
            // Arrange
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_WhenNotInDevelopment_ReturnsNotFound()
        {
            // Arrange
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Error_WhenMessageExists_ReturnsViewWithModel()
        {
            // Arrange
            var errorMessage = new ErrorMessage { ErrorDescription = "Test Error" };
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync(errorMessage);
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            var result = await _controller.Error("testErrorId");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(errorMessage, model.Error);
        }

        [Fact]
        public async Task Error_WhenNotInDevelopment_ClearsErrorDescription()
        {
            // Arrange
            var errorMessage = new ErrorMessage { ErrorDescription = "Test Error" };
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync(errorMessage);
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            var result = await _controller.Error("testErrorId");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Null(model.Error.ErrorDescription);
        }

        [Fact]
        public async Task Error_WhenMessageNotFound_ReturnsViewWithEmptyModel()
        {
            // Arrange
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync((ErrorMessage)null);

            // Act
            var result = await _controller.Error("testErrorId");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Null(model.Error);
        }
    }
}
