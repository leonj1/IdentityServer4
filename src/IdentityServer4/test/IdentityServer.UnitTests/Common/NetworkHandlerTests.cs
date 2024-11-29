using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class NetworkHandlerTests
    {
        [Fact]
        public async Task Should_Throw_Exception_When_Configured_With_Exception()
        {
            // Arrange
            var expectedException = new Exception("Test exception");
            var handler = new NetworkHandler(expectedException);
            var client = new HttpClient(handler);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                client.GetAsync("http://test.com"));
            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task Should_Return_Error_With_Status_Code_And_Reason()
        {
            // Arrange
            var statusCode = HttpStatusCode.BadRequest;
            var reason = "Bad Request Test";
            var handler = new NetworkHandler(statusCode, reason);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");

            // Assert
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(reason, response.ReasonPhrase);
        }

        [Fact]
        public async Task Should_Return_Document_With_Content()
        {
            // Arrange
            var document = "Test Document";
            var statusCode = HttpStatusCode.OK;
            var handler = new NetworkHandler(document, statusCode);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(document, content);
        }

        [Fact]
        public async Task Should_Use_Document_Selector()
        {
            // Arrange
            string DocumentSelector(HttpRequestMessage request) => 
                request.RequestUri.ToString();
            var handler = new NetworkHandler(DocumentSelector, HttpStatusCode.OK);
            var client = new HttpClient(handler);
            var testUrl = "http://test.com";

            // Act
            var response = await client.GetAsync(testUrl);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(testUrl, content);
        }

        [Fact]
        public async Task Should_Execute_Custom_Action()
        {
            // Arrange
            var customResponse = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("Custom Response")
            };
            var handler = new NetworkHandler(_ => customResponse);
            var client = new HttpClient(handler);

            // Act
            var response = await client.GetAsync("http://test.com");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Custom Response", content);
        }

        [Fact]
        public async Task Should_Capture_Request_And_Body()
        {
            // Arrange
            var handler = new NetworkHandler("test", HttpStatusCode.OK);
            var client = new HttpClient(handler);
            var content = new StringContent("Test Body");

            // Act
            await client.PostAsync("http://test.com", content);

            // Assert
            Assert.NotNull(handler.Request);
            Assert.Equal("Test Body", handler.Body);
        }
    }
}
