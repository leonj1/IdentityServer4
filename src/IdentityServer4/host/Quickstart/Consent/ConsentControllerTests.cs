using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class ConsentControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IEventService> _mockEvents;
        private readonly Mock<ILogger<ConsentController>> _mockLogger;
        private readonly ConsentController _controller;

        public ConsentControllerTests()
        {
            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockEvents = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<ConsentController>>();
            _controller = new ConsentController(_mockInteraction.Object, _mockEvents.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_Get_ReturnsErrorView_WhenNoValidRequest()
        {
            // Arrange
            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync((AuthorizationRequest)null);

            // Act
            var result = await _controller.Index("returnUrl");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Index_Get_ReturnsConsentView_WhenValidRequest()
        {
            // Arrange
            var request = new AuthorizationRequest
            {
                Client = new Client { ClientId = "client" },
                ValidatedResources = new ResourceValidationResult()
            };

            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(request);

            // Act
            var result = await _controller.Index("returnUrl");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.IsType<ConsentViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Index_Post_ReturnsDeniedResponse_WhenUserClicksNo()
        {
            // Arrange
            var request = new AuthorizationRequest
            {
                Client = new Client { ClientId = "client" },
                ValidatedResources = new ResourceValidationResult()
            };

            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(request);

            var model = new ConsentInputModel
            {
                Button = "no",
                ReturnUrl = "returnUrl"
            };

            // Act
            var result = await _controller.Index(model);

            // Assert
            _mockInteraction.Verify(x => x.GrantConsentAsync(
                It.IsAny<AuthorizationRequest>(),
                It.Is<ConsentResponse>(r => r.Error == AuthorizationError.AccessDenied)),
                Times.Once);
        }

        [Fact]
        public async Task Index_Post_ReturnsValidationError_WhenNoScopesSelected()
        {
            // Arrange
            var request = new AuthorizationRequest
            {
                Client = new Client { ClientId = "client" },
                ValidatedResources = new ResourceValidationResult()
            };

            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(request);

            var model = new ConsentInputModel
            {
                Button = "yes",
                ReturnUrl = "returnUrl",
                ScopesConsented = new string[] { }
            };

            // Act
            var result = await _controller.Index(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Contains(ConsentOptions.MustChooseOneErrorMessage, _controller.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}
