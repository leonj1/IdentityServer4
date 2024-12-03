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
            var context = new DefaultHttpContext();
            var actionContext = new ActionContext(
                context,
                new RouteData(),
                new ActionDescriptor()
            );
            
            var filters = new List<IFilterMetadata>();
            var viewResult = new ViewResult();
            
            var resultExecutingContext = new ResultExecutingContext(
                actionContext,
                filters,
                viewResult,
                controller: null
            );

            // Act
            attribute.OnResultExecuting(resultExecutingContext);

            // Assert
            var headers = context.Response.Headers;
            Assert.Equal("nosniff", headers["X-Content-Type-Options"]);
            Assert.Equal("SAMEORIGIN", headers["X-Frame-Options"]);
            Assert.Equal("no-referrer", headers["Referrer-Policy"]);
            
            var expectedCsp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
            Assert.Equal(expectedCsp, headers["Content-Security-Policy"]);
            Assert.Equal(expectedCsp, headers["X-Content-Security-Policy"]);
        }

        [Fact]
        public void OnResultExecuting_WhenNotViewResult_DoesNotAddHeaders()
        {
            // Arrange
            var attribute = new SecurityHeadersAttribute();
            var context = new DefaultHttpContext();
            var actionContext = new ActionContext(
                context,
                new RouteData(),
                new ActionDescriptor()
            );
            
            var filters = new List<IFilterMetadata>();
            var jsonResult = new JsonResult(new { });
            
            var resultExecutingContext = new ResultExecutingContext(
                actionContext,
                filters,
                jsonResult,
                controller: null
            );

            // Act
            attribute.OnResultExecuting(resultExecutingContext);

            // Assert
            var headers = context.Response.Headers;
            Assert.Empty(headers);
        }
    }
}
