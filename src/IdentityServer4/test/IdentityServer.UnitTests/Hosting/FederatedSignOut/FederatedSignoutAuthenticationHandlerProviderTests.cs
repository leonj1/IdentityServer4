using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Hosting.FederatedSignOut;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Hosting.FederatedSignOut
{
    public class FederatedSignoutAuthenticationHandlerProviderTests
    {
        private readonly Mock<IAuthenticationHandlerProvider> _mockProvider;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly FederatedSignoutAuthenticationHandlerProvider _subject;

        public FederatedSignoutAuthenticationHandlerProviderTests()
        {
            _mockProvider = new Mock<IAuthenticationHandlerProvider>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            
            var decorator = new Decorator<IAuthenticationHandlerProvider>(_mockProvider.Object);
            _subject = new FederatedSignoutAuthenticationHandlerProvider(decorator, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetHandlerAsync_WhenHandlerIsNotRequestHandler_ReturnsOriginalHandler()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "test";
            var mockHandler = new Mock<IAuthenticationHandler>();
            
            _mockProvider.Setup(x => x.GetHandlerAsync(context, scheme))
                .ReturnsAsync(mockHandler.Object);

            // Act
            var result = await _subject.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().Be(mockHandler.Object);
        }

        [Fact]
        public async Task GetHandlerAsync_WhenHandlerIsSignInHandler_ReturnsWrappedHandler()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "test";
            var mockHandler = new Mock<IAuthenticationSignInHandler>();
            
            _mockProvider.Setup(x => x.GetHandlerAsync(context, scheme))
                .ReturnsAsync(mockHandler.Object);

            // Act
            var result = await _subject.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().BeOfType<AuthenticationRequestSignInHandlerWrapper>();
        }

        [Fact]
        public async Task GetHandlerAsync_WhenHandlerIsSignOutHandler_ReturnsWrappedHandler()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "test";
            var mockHandler = new Mock<IAuthenticationSignOutHandler>();
            
            _mockProvider.Setup(x => x.GetHandlerAsync(context, scheme))
                .ReturnsAsync(mockHandler.Object);

            // Act
            var result = await _subject.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().BeOfType<AuthenticationRequestSignOutHandlerWrapper>();
        }

        [Fact]
        public async Task GetHandlerAsync_WhenHandlerIsRequestHandler_ReturnsWrappedHandler()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var scheme = "test";
            var mockHandler = new Mock<IAuthenticationRequestHandler>();
            
            _mockProvider.Setup(x => x.GetHandlerAsync(context, scheme))
                .ReturnsAsync(mockHandler.Object);

            // Act
            var result = await _subject.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().BeOfType<AuthenticationRequestHandlerWrapper>();
        }
    }
}
