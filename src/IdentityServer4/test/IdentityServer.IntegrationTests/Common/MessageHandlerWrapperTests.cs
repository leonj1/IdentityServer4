using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.IntegrationTests.Common
{
    public class MessageHandlerWrapperTests
    {
        [Fact]
        public async Task SendAsync_ShouldDelegateAndCaptureResponse()
        {
            // Arrange
            var innerResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var innerHandler = new TestHandler(innerResponse);
            var wrapper = new MessageHandlerWrapper(innerHandler);
            var client = new HttpClient(wrapper);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Same(innerResponse, response);
            Assert.Same(innerResponse, wrapper.Response);
        }

        [Fact]
        public async Task SendAsync_ShouldRespectCancellation()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var innerHandler = new TestHandler(new HttpResponseMessage(), delay: TimeSpan.FromSeconds(1));
            var wrapper = new MessageHandlerWrapper(innerHandler);
            var client = new HttpClient(wrapper);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act & Assert
            cts.Cancel();
            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                client.SendAsync(request, cts.Token));
        }

        private class TestHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;
            private readonly TimeSpan _delay;

            public TestHandler(HttpResponseMessage response, TimeSpan? delay = null)
            {
                _response = response;
                _delay = delay ?? TimeSpan.Zero;
            }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, 
                CancellationToken cancellationToken)
            {
                await Task.Delay(_delay, cancellationToken);
                return _response;
            }
        }
    }
}
