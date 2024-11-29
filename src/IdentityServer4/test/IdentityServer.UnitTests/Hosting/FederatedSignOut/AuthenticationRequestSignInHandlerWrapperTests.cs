using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Hosting.FederatedSignOut;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Hosting.FederatedSignOut
{
    public class AuthenticationRequestSignInHandlerWrapperTests
    {
        private readonly Mock<IAuthenticationSignInHandler> _mockInnerHandler;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly AuthenticationRequestSignInHandlerWrapper _subject;

        public AuthenticationRequestSignInHandlerWrapperTests()
        {
            _mockInnerHandler = new Mock<IAuthenticationSignInHandler>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _subject = new AuthenticationRequestSignInHandlerWrapper(_mockInnerHandler.Object, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task SignInAsync_ShouldDelegateToInnerHandler()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var properties = new AuthenticationProperties();

            // Act
            await _subject.SignInAsync(principal, properties);

            // Assert
            _mockInnerHandler.Verify(x => x.SignInAsync(principal, properties), Times.Once);
        }

        [Fact]
        public async Task SignOutAsync_ShouldDelegateToInnerHandler()
        {
            // Arrange
            var properties = new AuthenticationProperties();

            // Act
            await _subject.SignOutAsync(properties);

            // Assert
            _mockInnerHandler.Verify(x => x.SignOutAsync(properties), Times.Once);
        }

        [Fact]
        public void Instance_ShouldImplementCorrectInterfaces()
        {
            // Assert
            _subject.Should().BeAssignableTo<IAuthenticationRequestHandler>();
            _subject.Should().BeAssignableTo<IAuthenticationSignInHandler>();
        }
    }
}
