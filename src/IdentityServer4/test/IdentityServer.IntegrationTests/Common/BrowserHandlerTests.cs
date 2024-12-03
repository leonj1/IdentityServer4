using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.IntegrationTests.Common
{
    public class BrowserHandlerTests
    {
        private class TestHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;
            public HttpRequestMessage LastRequest { get; private set; }

            public TestHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(_response);
            }
        }

        [Fact]
        public async Task Should_Handle_Cookies_When_Enabled()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Set-Cookie", "test=value");
            
            var testHandler = new TestHandler(response);
            var handler = new BrowserHandler(testHandler);
            var client = new HttpClient(handler);

            // Act
            await client.GetAsync("http://example.com");
            var cookie = handler.GetCookie("http://example.com", "test");

            // Assert
            Assert.NotNull(cookie);
            Assert.Equal("value", cookie.Value);
        }

        [Fact]
        public async Task Should_Not_Handle_Cookies_When_Disabled()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Set-Cookie", "test=value");
            
            var testHandler = new TestHandler(response);
            var handler = new BrowserHandler(testHandler) { AllowCookies = false };
            var client = new HttpClient(handler);

            // Act
            await client.GetAsync("http://example.com");
            var cookie = handler.GetCookie("http://example.com", "test");

            // Assert
            Assert.Null(cookie);
        }

        [Fact]
        public async Task Should_Follow_Redirects_When_Enabled()
        {
            // Arrange
            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("http://example.com/redirected");
            
            var finalResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var redirectCount = 0;
            
            var testHandler = new TestHandler(redirectResponse);
            var handler = new BrowserHandler(testHandler);
            var client = new HttpClient(handler);

            // Act & Assert
            await client.GetAsync("http://example.com");
            Assert.Equal("http://example.com/redirected", testHandler.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task Should_Throw_On_Too_Many_Redirects()
        {
            // Arrange
            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("http://example.com/redirect");
            
            var testHandler = new TestHandler(redirectResponse);
            var handler = new BrowserHandler(testHandler) { ErrorRedirectLimit = 5 };
            var client = new HttpClient(handler);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                client.GetAsync("http://example.com"));
        }

        [Fact]
        public async Task Should_Remove_Cookie_When_Requested()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Set-Cookie", "test=value");
            
            var testHandler = new TestHandler(response);
            var handler = new BrowserHandler(testHandler);
            var client = new HttpClient(handler);

            // Act
            await client.GetAsync("http://example.com");
            handler.RemoveCookie("http://example.com", "test");
            var cookie = handler.GetCookie("http://example.com", "test");

            // Assert
            Assert.Null(cookie);
        }
    }
}
