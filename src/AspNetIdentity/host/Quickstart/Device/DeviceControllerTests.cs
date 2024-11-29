using System;
using System.Collections.Generic;
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
            _controller.Request.Query = new Microsoft.AspNetCore.Http.QueryCollection();

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UserCodeCapture", result.ViewName);
        }

        [Fact]
        public async Task UserCodeCapture_WithValidCode_ReturnsUserCodeConfirmationView()
        {
            // Arrange
            var userCode = "valid_code";
            var authRequest = new DeviceFlowAuthorizationRequest();
            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(userCode))
                .ReturnsAsync(authRequest);

            // Act
            var result = await _controller.UserCodeCapture(userCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UserCodeConfirmation", result.ViewName);
        }

        [Fact]
        public async Task Callback_WithNoModel_ThrowsArgumentNullException()
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
                UserCode = "test_code"
            };

            var authRequest = new DeviceFlowAuthorizationRequest
            {
                Client = new Client(),
                ValidatedResources = new ResourceValidationResult()
            };

            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(model.UserCode))
                .ReturnsAsync(authRequest);

            // Act
            var result = await _controller.Callback(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.ViewName);
            _mockEventService.Verify(x => x.RaiseAsync(It.IsAny<ConsentDeniedEvent>()), Times.Once);
        }
    }
}
