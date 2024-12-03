using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class BackChannelLogoutHttpClientTests
    {
        private readonly IBackChannelLogoutHttpClient _client;
        private readonly Mock<HttpClient> _mockHttpClient;

        public BackChannelLogoutHttpClientTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _client = new DefaultBackChannelLogoutHttpClient(_mockHttpClient.Object);
        }

        [Fact]
        public async Task PostAsync_ShouldSendHttpPostRequest()
        {
            // Arrange
            var url = "https://test.com/logout";
            var payload = new Dictionary<string, string>
            {
                { "logoutToken", "token123" }
            };

            // Act
            await _client.PostAsync(url, payload);

            // Assert
            _mockHttpClient.Verify(x => x.PostAsync(
                It.Is<string>(s => s == url),
                It.IsAny<HttpContent>()),
                Times.Once);
        }

        [Fact]
        public async Task PostAsync_WithNullUrl_ShouldThrowArgumentNullException()
        {
            // Arrange
            string url = null;
            var payload = new Dictionary<string, string>();

            // Act
            Func<Task> act = async () => await _client.PostAsync(url, payload);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task PostAsync_WithNullPayload_ShouldThrowArgumentNullException()
        {
            // Arrange
            var url = "https://test.com/logout";
            Dictionary<string, string> payload = null;

            // Act
            Func<Task> act = async () => await _client.PostAsync(url, payload);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
