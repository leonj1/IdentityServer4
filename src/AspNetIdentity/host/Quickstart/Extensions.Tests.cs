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
            var controller = new Mock<Controller>();
            var httpContext = new DefaultHttpContext();
            controller.Setup(c => c.HttpContext).Returns(httpContext);

            const string viewName = "LoadingPage";
            const string redirectUri = "https://example.com";

            // Act
            var result = controller.Object.LoadingPage(viewName, redirectUri) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(viewName, result.ViewName);
            
            var model = result.Model as RedirectViewModel;
            Assert.NotNull(model);
            Assert.Equal(redirectUri, model.RedirectUrl);
            
            Assert.Equal(200, httpContext.Response.StatusCode);
            Assert.Equal(string.Empty, httpContext.Response.Headers["Location"]);
        }
    }
}
