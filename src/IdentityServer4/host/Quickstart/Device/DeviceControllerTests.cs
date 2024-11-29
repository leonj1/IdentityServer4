using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class DeviceControllerTests
    {
        private readonly Mock<IDeviceFlowInteractionService> _mockInteraction;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<IOptions<IdentityServerOptions>> _mockOptions;
        private readonly Mock<ILogger<DeviceController>> _mockLogger;
        private readonly DeviceController _controller;

        public DeviceControllerTests()
        {
            _mockInteraction = new Mock<IDeviceFlowInteractionService>();
            _mockEventService = new Mock<IEventService>();
            _mockOptions = new Mock<IOptions<IdentityServerOptions>>();
            _mockLogger = new Mock<ILogger<DeviceController>>();

            _mockOptions.Setup(x => x.Value).Returns(new IdentityServerOptions());

            _controller = new DeviceController(
                _mockInteraction.Object,
                _mockEventService.Object,
                _mockOptions.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Index_WithNoUserCode_ReturnsUserCodeCaptureView()
        {
            // Arrange
            _controller.Request = new Microsoft.AspNetCore.Http.DefaultHttpRequest(new Microsoft.AspNetCore.Http.DefaultHttpContext());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("UserCodeCapture", viewResult.ViewName);
        }

        [Fact]
        public async Task UserCodeCapture_WithValidCode_ReturnsUserCodeConfirmationView()
        {
            // Arrange
            var userCode = "123456";
            var request = new DeviceFlowAuthorizationRequest
            {
                Client = new Client { ClientId = "test-client" },
                ValidatedResources = new ResourceValidationResult()
            };
            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(userCode))
                .ReturnsAsync(request);

            // Act
            var result = await _controller.UserCodeCapture(userCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("UserCodeConfirmation", viewResult.ViewName);
        }

        [Fact]
        public async Task Callback_WithNullModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.Callback(null));
        }

        [Fact]
        public async Task Callback_WhenUserDeniesConsent_ReturnsSuccessView()
        {
            // Arrange
            var model = new DeviceAuthorizationInputModel
            {
                Button = "no",
                UserCode = "123456"
            };
            var request = new DeviceFlowAuthorizationRequest
            {
                Client = new Client { ClientId = "test-client" },
                ValidatedResources = new ResourceValidationResult()
            };
            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(model.UserCode))
                .ReturnsAsync(request);

            // Act
            var result = await _controller.Callback(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Success", viewResult.ViewName);
        }
    }
}
