using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.IntegrationTests.Common
{
    public class NetworkHandlerTests
    {
        [Fact]
        public async Task WhenConstructedWithException_ShouldThrowOnSend()
        {
            // Arrange
            var expectedException = new Exception("test exception");
            var handler = new NetworkHandler(expectedException);
            var client = new HttpClient(handler);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                client.GetAsync("http://test.com"));
            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task WhenConstructedWithStatusAndReason_ShouldReturnErrorResponse()
        {
            // Arrange
            var expectedStatus = HttpStatusCode.BadRequest;
            var expectedReason = "test reason";
            var handler = new NetworkHandler(expectedStatus, expectedReason);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");

            // Assert
            Assert.Equal(expectedStatus, response.StatusCode);
            Assert.Equal(expectedReason, response.ReasonPhrase);
        }

        [Fact]
        public async Task WhenConstructedWithDocument_ShouldReturnDocumentContent()
        {
            // Arrange
            var expectedContent = "test document";
            var expectedStatus = HttpStatusCode.OK;
            var handler = new NetworkHandler(expectedContent, expectedStatus);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedStatus, response.StatusCode);
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public async Task WhenConstructedWithDocumentSelector_ShouldUseSelector()
        {
            // Arrange
            var expectedContent = "selected content";
            var handler = new NetworkHandler(
                req => expectedContent,
                HttpStatusCode.OK);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public async Task WhenConstructedWithAction_ShouldInvokeAction()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.Created);
            var handler = new NetworkHandler(req => expectedResponse);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");

            // Assert
            Assert.Same(expectedResponse, response);
        }

        [Fact]
        public async Task ShouldCaptureRequestAndBody()
        {
            // Arrange
            var handler = new NetworkHandler(HttpStatusCode.OK, "OK");
            var client = new HttpClient(handler);
            var content = new StringContent("test body");

            // Act
            await client.PostAsync("http://test.com", content);

            // Assert
            Assert.NotNull(handler.Request);
            Assert.Equal("test body", handler.Body);
        }
    }
}
