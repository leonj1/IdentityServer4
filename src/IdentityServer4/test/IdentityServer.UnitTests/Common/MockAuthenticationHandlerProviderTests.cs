using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;
using FluentAssertions;
using Moq;

namespace IdentityServer.UnitTests.Common
{
    public class MockAuthenticationHandlerProviderTests
    {
        [Fact]
        public async Task GetHandlerAsync_ShouldReturnConfiguredHandler()
        {
            // Arrange
            var mockHandler = new Mock<IAuthenticationHandler>();
            var provider = new MockAuthenticationHandlerProvider
            {
                Handler = mockHandler.Object
            };
            var context = new DefaultHttpContext();
            var scheme = "TestScheme";

            // Act
            var result = await provider.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().BeSameAs(mockHandler.Object);
        }

        [Fact]
        public async Task GetHandlerAsync_WhenHandlerNotSet_ShouldReturnNull()
        {
            // Arrange
            var provider = new MockAuthenticationHandlerProvider();
            var context = new DefaultHttpContext();
            var scheme = "TestScheme";

            // Act
            var result = await provider.GetHandlerAsync(context, scheme);

            // Assert
            result.Should().BeNull();
        }
    }
}
