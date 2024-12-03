using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI.Tests
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
                _loggerMock.Object
            );
        }

        [Fact]
        public void Index_WhenDevelopment_ReturnsViewResult()
        {
            // Arrange
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_WhenProduction_ReturnsNotFound()
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
            var errorMessage = new ErrorMessage { Error = "test_error", ErrorDescription = "test description" };
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync(errorMessage);
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            var result = await _controller.Error("error_id");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(errorMessage, model.Error);
        }

        [Fact]
        public async Task Error_InProduction_HidesErrorDescription()
        {
            // Arrange
            var errorMessage = new ErrorMessage { Error = "test_error", ErrorDescription = "test description" };
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync(errorMessage);
            _environmentMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            var result = await _controller.Error("error_id");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Null(model.Error.ErrorDescription);
        }

        [Fact]
        public async Task Error_WhenMessageNotFound_ReturnsEmptyViewModel()
        {
            // Arrange
            _interactionMock.Setup(i => i.GetErrorContextAsync(It.IsAny<string>()))
                .ReturnsAsync((ErrorMessage)null);

            // Act
            var result = await _controller.Error("error_id");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Null(model.Error);
        }
    }
}
