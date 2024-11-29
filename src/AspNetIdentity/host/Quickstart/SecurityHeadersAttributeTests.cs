using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class SecurityHeadersAttributeTests
    {
        [Fact]
        public void OnResultExecuting_WhenViewResult_AddsSecurityHeaders()
        {
            // Arrange
            var attribute = new SecurityHeadersAttribute();
            var context = new ResultExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new ViewResult(),
                new object()
            );

            // Act
            attribute.OnResultExecuting(context);

            // Assert
            var headers = context.HttpContext.Response.Headers;
            Assert.Equal("nosniff", headers["X-Content-Type-Options"]);
            Assert.Equal("SAMEORIGIN", headers["X-Frame-Options"]);
            Assert.Contains("default-src 'self'", headers["Content-Security-Policy"].ToString());
            Assert.Contains("default-src 'self'", headers["X-Content-Security-Policy"].ToString());
            Assert.Equal("no-referrer", headers["Referrer-Policy"]);
        }

        [Fact]
        public void OnResultExecuting_WhenNotViewResult_DoesNotAddHeaders()
        {
            // Arrange
            var attribute = new SecurityHeadersAttribute();
            var context = new ResultExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new ContentResult(), // Not a ViewResult
                new object()
            );

            // Act
            attribute.OnResultExecuting(context);

            // Assert
            var headers = context.HttpContext.Response.Headers;
            Assert.Empty(headers);
        }
    }
}
