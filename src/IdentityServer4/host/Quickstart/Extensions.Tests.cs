using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI.Tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData("https://example.com", false)]
        [InlineData("http://example.com", false)]
        [InlineData("myapp://callback", true)]
        [InlineData("customscheme:callback", true)]
        public void IsNativeClient_ShouldCorrectlyIdentifyNativeClients(string redirectUri, bool expected)
        {
            // Arrange
            var context = new AuthorizationRequest { RedirectUri = redirectUri };

            // Act
            var result = context.IsNativeClient();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LoadingPage_ShouldReturnCorrectViewModel()
        {
            // Arrange
            var redirectUri = "https://example.com";
            var viewName = "LoadingPage";

            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var controller = new Mock<Controller>();
            controller.Setup(c => c.HttpContext).Returns(httpContext);
            controller.Setup(c => c.View(It.IsAny<string>(), It.IsAny<RedirectViewModel>()))
                     .Returns((string view, RedirectViewModel model) => new ViewResult { ViewName = view, ViewData = { Model = model } });

            // Act
            var result = controller.Object.LoadingPage(viewName, redirectUri) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(viewName, result.ViewName);
            Assert.Equal(200, httpContext.Response.StatusCode);
            Assert.Empty(httpContext.Response.Headers["Location"]);
            
            var model = result.ViewData.Model as RedirectViewModel;
            Assert.NotNull(model);
            Assert.Equal(redirectUri, model.RedirectUrl);
        }
    }
}
