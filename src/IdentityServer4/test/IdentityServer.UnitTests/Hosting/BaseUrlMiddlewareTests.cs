using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Hosting
{
    public class BaseUrlMiddlewareTests
    {
        private readonly IdentityServerOptions _options;

        public BaseUrlMiddlewareTests()
        {
            _options = new IdentityServerOptions();
        }

        [Fact]
        public async Task StandardBasePath_Should_SetCorrectBasePath()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.PathBase = new PathString("/base");
            var nextCalled = false;

            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                ctx.Items["IdentityServerBasePath"].Should().Be("/base");
                return Task.CompletedTask;
            };

            var middleware = new BaseUrlMiddleware(next, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task BasePathWithTrailingSlash_Should_RemoveTrailingSlash()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.PathBase = new PathString("/base/");
            var nextCalled = false;

            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                ctx.Items["IdentityServerBasePath"].Should().Be("/base");
                return Task.CompletedTask;
            };

            var middleware = new BaseUrlMiddleware(next, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task EmptyBasePath_Should_SetEmptyBasePath()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.PathBase = new PathString("");
            var nextCalled = false;

            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                ctx.Items["IdentityServerBasePath"].Should().Be("");
                return Task.CompletedTask;
            };

            var middleware = new BaseUrlMiddleware(next, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }
    }
}
