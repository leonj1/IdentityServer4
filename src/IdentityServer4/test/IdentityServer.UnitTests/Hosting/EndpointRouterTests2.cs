using System;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Hosting
{
    public class EndpointRouterTests2
    {
        private readonly Mock<ILogger<EndpointRouter>> _logger;
        private readonly IdentityServerOptions _options;
        private readonly List<Endpoint> _endpoints;

        public EndpointRouterTests2()
        {
            _logger = new Mock<ILogger<EndpointRouter>>();
            _options = new IdentityServerOptions();
            _endpoints = new List<Endpoint>();
        }

        [Fact]
        public void Find_WhenContextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var router = new EndpointRouter(_endpoints, _options, _logger.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => router.Find(null));
        }

        [Fact]
        public void Find_WhenPathMatches_ReturnsHandler()
        {
            // Arrange
            var handlerType = typeof(TestEndpointHandler);
            var endpoint = new Endpoint("test", "/test", handlerType);
            _endpoints.Add(endpoint);

            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            
            var handler = new TestEndpointHandler();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(handlerType)).Returns(handler);
            context.RequestServices = serviceProvider.Object;

            var router = new EndpointRouter(_endpoints, _options, _logger.Object);

            // Act
            var result = router.Find(context);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestEndpointHandler>();
        }

        [Fact]
        public void Find_WhenEndpointDisabled_ReturnsNull()
        {
            // Arrange
            var handlerType = typeof(TestEndpointHandler);
            var endpoint = new Endpoint("test", "/test", handlerType);
            _endpoints.Add(endpoint);
            
            _options.Endpoints.EnableTestEndpoint = false;

            var context = new DefaultHttpContext();
            context.Request.Path = "/test";

            var router = new EndpointRouter(_endpoints, _options, _logger.Object);

            // Act
            var result = router.Find(context);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Find_WhenPathDoesNotMatch_ReturnsNull()
        {
            // Arrange
            var handlerType = typeof(TestEndpointHandler);
            var endpoint = new Endpoint("test", "/test", handlerType);
            _endpoints.Add(endpoint);

            var context = new DefaultHttpContext();
            context.Request.Path = "/nonexistent";

            var router = new EndpointRouter(_endpoints, _options, _logger.Object);

            // Act
            var result = router.Find(context);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Find_WhenServiceResolutionFails_ReturnsNull()
        {
            // Arrange
            var handlerType = typeof(TestEndpointHandler);
            var endpoint = new Endpoint("test", "/test", handlerType);
            _endpoints.Add(endpoint);

            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(handlerType)).Returns(null);
            context.RequestServices = serviceProvider.Object;

            var router = new EndpointRouter(_endpoints, _options, _logger.Object);

            // Act
            var result = router.Find(context);

            // Assert
            result.Should().BeNull();
        }

        private class TestEndpointHandler : IEndpointHandler
        {
            public Task<IEndpointResult> ProcessAsync(HttpContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
