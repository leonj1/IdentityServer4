using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System.Threading.Tasks;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer4.UnitTests.Quickstart
{
    public class AccountControllerTests
    {
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<IEventService> _mockEvents;
        private readonly TestUserStore _users;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _mockEvents = new Mock<IEventService>();
            _users = new TestUserStore(TestUsers.Users);

            _controller = new AccountController(
                _mockInteraction.Object,
                _mockClientStore.Object,
                _mockSchemeProvider.Object,
                _mockEvents.Object,
                _users);

            // Setup controller context
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _controller.Url = mockUrlHelper.Object;

            var mockHttpContext = new Mock<HttpContext>();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Login_Get_ReturnsViewWithModel()
        {
            // Arrange
            var returnUrl = "/return";

            // Act
            var result = await _controller.Login(returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.Equal(returnUrl, model.ReturnUrl);
        }

        [Fact]
        public async Task Login_Post_WithValidCredentials_RedirectsToReturnUrl()
        {
            // Arrange
            var model = new LoginInputModel
            {
                Username = "alice",
                Password = "alice",
                ReturnUrl = "/return"
            };

            // Act
            var result = await _controller.Login(model, "login");

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(model.ReturnUrl, redirectResult.Url);
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginInputModel
            {
                Username = "invalid",
                Password = "invalid",
                ReturnUrl = "/return"
            };

            // Act
            var result = await _controller.Login(model, "login");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }

        [Fact]
        public async Task Logout_Get_ReturnsViewWithModel()
        {
            // Arrange
            var logoutId = "logoutId";

            // Act
            var result = await _controller.Logout(logoutId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LogoutViewModel>(viewResult.Model);
            Assert.Equal(logoutId, model.LogoutId);
        }

        [Fact]
        public async Task AccessDenied_ReturnsView()
        {
            // Act
            var result = _controller.AccessDenied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
