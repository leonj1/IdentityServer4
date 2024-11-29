using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServerHost.Tests.Quickstart.UI
{
    public class ExternalControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IEventService> _mockEvents;
        private readonly Mock<ILogger<ExternalController>> _mockLogger;
        private readonly ExternalController _controller;

        public ExternalControllerTests()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockEvents = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<ExternalController>>();

            _controller = new ExternalController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockInteraction.Object,
                _mockClientStore.Object,
                _mockEvents.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Challenge_WithEmptyReturnUrl_SetsDefaultReturnUrl()
        {
            // Arrange
            var scheme = "TestScheme";
            
            // Act
            var result = _controller.Challenge(scheme, returnUrl: null) as ChallengeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(scheme, result.AuthenticationSchemes.First());
            var props = result.Properties;
            Assert.Equal("~/", props.Items["returnUrl"]);
        }

        [Fact]
        public void Challenge_WithInvalidReturnUrl_ThrowsException()
        {
            // Arrange
            var scheme = "TestScheme";
            var invalidReturnUrl = "http://malicious.com";
            _mockInteraction.Setup(x => x.IsValidReturnUrl(invalidReturnUrl)).Returns(false);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => 
                _controller.Challenge(scheme, invalidReturnUrl));
            Assert.Equal("invalid return URL", exception.Message);
        }

        [Fact]
        public void Challenge_WithValidReturnUrl_ReturnsChallenge()
        {
            // Arrange
            var scheme = "TestScheme";
            var validReturnUrl = "/valid";
            _mockInteraction.Setup(x => x.IsValidReturnUrl(validReturnUrl)).Returns(true);

            // Act
            var result = _controller.Challenge(scheme, validReturnUrl) as ChallengeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(scheme, result.AuthenticationSchemes.First());
            var props = result.Properties;
            Assert.Equal(validReturnUrl, props.Items["returnUrl"]);
            Assert.Equal(scheme, props.Items["scheme"]);
        }
    }
}
