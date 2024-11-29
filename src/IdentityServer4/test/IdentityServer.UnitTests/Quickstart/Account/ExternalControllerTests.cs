using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServerHost.Quickstart.UI;
using Xunit;

namespace IdentityServer.UnitTests.Quickstart.Account
{
    public class ExternalControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IEventService> _mockEvents;
        private readonly Mock<ILogger<ExternalController>> _mockLogger;
        private readonly TestUserStore _users;
        private readonly ExternalController _controller;

        public ExternalControllerTests()
        {
            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockEvents = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<ExternalController>>();
            _users = new TestUserStore(TestUsers.Users);
            
            _controller = new ExternalController(
                _mockInteraction.Object,
                _mockClientStore.Object,
                _mockEvents.Object,
                _mockLogger.Object,
                _users);
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
            var ex = Assert.Throws<Exception>(() => _controller.Challenge(scheme, invalidReturnUrl));
            Assert.Equal("invalid return URL", ex.Message);
        }

        [Fact]
        public async Task Callback_WithFailedAuthentication_ThrowsException()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Callback());
        }

        [Fact]
        public void FindUserFromExternalProvider_WithoutUserIdClaim_ThrowsException()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("email", "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties();
            var result = AuthenticateResult.Success(new AuthenticationTicket(principal, properties, "Test"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.GetType()
                .GetMethod("FindUserFromExternalProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_controller, new object[] { result }));
        }
    }
}
