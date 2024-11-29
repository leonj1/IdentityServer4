using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Hosting.FederatedSignOut;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Hosting.FederatedSignOut
{
    public class AuthenticationRequestSignOutHandlerWrapperTests
    {
        private readonly Mock<IAuthenticationSignOutHandler> _mockInnerHandler;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly AuthenticationRequestSignOutHandlerWrapper _wrapper;
        private readonly AuthenticationProperties _testProperties;

        public AuthenticationRequestSignOutHandlerWrapperTests()
        {
            _mockInnerHandler = new Mock<IAuthenticationSignOutHandler>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _wrapper = new AuthenticationRequestSignOutHandlerWrapper(_mockInnerHandler.Object, _mockHttpContextAccessor.Object);
            _testProperties = new AuthenticationProperties();
        }

        [Fact]
        public async Task SignOutAsync_ShouldDelegateToInnerHandler()
        {
            // Arrange
            _mockInnerHandler.Setup(x => x.SignOutAsync(_testProperties))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _wrapper.SignOutAsync(_testProperties);

            // Assert
            _mockInnerHandler.Verify();
        }

        [Fact]
        public async Task HandleRequestAsync_ShouldDelegateToInnerHandler()
        {
            // Arrange
            _mockInnerHandler.Setup(x => x.HandleRequestAsync())
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            var result = await _wrapper.HandleRequestAsync();

            // Assert
            result.Should().BeTrue();
            _mockInnerHandler.Verify();
        }
    }
}
