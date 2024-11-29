using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Hosting
{
    public class IEndpointHandlerTests
    {
        [Fact]
        public async Task ProcessAsync_ShouldReturnEndpointResult()
        {
            // Arrange
            var mockEndpointHandler = new Mock<IEndpointHandler>();
            var mockEndpointResult = new Mock<IEndpointResult>();
            var httpContext = new DefaultHttpContext();

            mockEndpointHandler
                .Setup(h => h.ProcessAsync(httpContext))
                .ReturnsAsync(mockEndpointResult.Object);

            // Act
            var result = await mockEndpointHandler.Object.ProcessAsync(httpContext);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(mockEndpointResult.Object);
            mockEndpointHandler.Verify(h => h.ProcessAsync(httpContext), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new TestEndpointHandler();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                handler.ProcessAsync(null));
        }

        private class TestEndpointHandler : IEndpointHandler
        {
            public Task<IEndpointResult> ProcessAsync(HttpContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));
                    
                return Task.FromResult<IEndpointResult>(null);
            }
        }
    }
}
