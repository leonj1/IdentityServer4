using Microsoft.AspNetCore.Http;
using Xunit;
using FluentAssertions;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Moq;

namespace IdentityServer.UnitTests.Common
{
    public class MockHttpContextAccessorTests
    {
        [Fact]
        public void Constructor_WithDefaultParameters_ShouldInitializeCorrectly()
        {
            // Act
            var accessor = new MockHttpContextAccessor();

            // Assert
            accessor.Should().NotBeNull();
            accessor.HttpContext.Should().NotBeNull();
            accessor.AuthenticationService.Should().NotBeNull();
            accessor.Schemes.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithCustomOptions_ShouldUseProvidedOptions()
        {
            // Arrange
            var options = TestIdentityServerOptions.Create();

            // Act
            var accessor = new MockHttpContextAccessor(options);

            // Assert
            accessor.Should().NotBeNull();
            accessor.HttpContext.RequestServices.GetService(typeof(IdentityServer4.Configuration.IdentityServerOptions))
                .Should().BeSameAs(options);
        }

        [Fact]
        public void Constructor_WithCustomUserSession_ShouldUseProvidedSession()
        {
            // Arrange
            var mockUserSession = new Mock<IUserSession>();

            // Act
            var accessor = new MockHttpContextAccessor(userSession: mockUserSession.Object);

            // Assert
            accessor.HttpContext.RequestServices.GetService(typeof(IUserSession))
                .Should().BeSameAs(mockUserSession.Object);
        }

        [Fact]
        public void Constructor_WithCustomEndSessionStore_ShouldUseProvidedStore()
        {
            // Arrange
            var mockStore = new Mock<IMessageStore<LogoutNotificationContext>>();

            // Act
            var accessor = new MockHttpContextAccessor(endSessionStore: mockStore.Object);

            // Assert
            accessor.HttpContext.RequestServices.GetService(typeof(IMessageStore<LogoutNotificationContext>))
                .Should().BeSameAs(mockStore.Object);
        }

        [Fact]
        public void HttpContext_SetAndGet_ShouldWorkCorrectly()
        {
            // Arrange
            var accessor = new MockHttpContextAccessor();
            var newContext = new DefaultHttpContext();

            // Act
            accessor.HttpContext = newContext;

            // Assert
            accessor.HttpContext.Should().BeSameAs(newContext);
        }
    }
}
