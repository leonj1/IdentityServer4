using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using IdentityServerHost.Quickstart.UI;

namespace IdentityServer.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<IEventService> _mockEvents;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _mockEvents = new Mock<IEventService>();

            _controller = new AccountController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockInteraction.Object,
                _mockClientStore.Object,
                _mockSchemeProvider.Object,
                _mockEvents.Object);
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
                Username = "testuser",
                Password = "password123",
                ReturnUrl = "/return"
            };

            _mockSignInManager
                .Setup(x => x.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var user = new ApplicationUser { UserName = model.Username, Id = "testid" };
            _mockUserManager
                .Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(user);

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
                Username = "testuser",
                Password = "wrongpassword",
                ReturnUrl = "/return"
            };

            _mockSignInManager
                .Setup(x => x.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _controller.Login(model, "login");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Logout_Get_ReturnsViewWithModel()
        {
            // Arrange
            var logoutId = "testlogoutid";

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
