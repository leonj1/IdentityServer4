using System;
using FluentAssertions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Hosting
{
    public class IEndpointRouterTests
    {
        private readonly Mock<IEndpointRouter> _mockRouter;
        private readonly Mock<IEndpointHandler> _mockHandler;
        private readonly HttpContext _httpContext;

        public IEndpointRouterTests()
        {
            _mockRouter = new Mock<IEndpointRouter>();
            _mockHandler = new Mock<IEndpointHandler>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void Find_WhenContextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            _mockRouter.Setup(r => r.Find(null))
                .Throws<ArgumentNullException>();

            // Act
            Action act = () => _mockRouter.Object.Find(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Find_WhenValidContext_ShouldReturnEndpointHandler()
        {
            // Arrange
            _mockRouter.Setup(r => r.Find(_httpContext))
                .Returns(_mockHandler.Object);

            // Act
            var result = _mockRouter.Object.Find(_httpContext);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(_mockHandler.Object);
        }

        [Fact]
        public void Find_WhenNoMatchingEndpoint_ShouldReturnNull()
        {
            // Arrange
            _mockRouter.Setup(r => r.Find(_httpContext))
                .Returns((IEndpointHandler)null);

            // Act
            var result = _mockRouter.Object.Find(_httpContext);

            // Assert
            result.Should().BeNull();
        }
    }
}
