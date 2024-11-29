using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class BackChannelLogoutHttpClientTests
    {
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<ILogger<DefaultBackChannelLogoutHttpClient>> _logger;
        private readonly Mock<HttpMessageHandler> _httpHandler;
        private readonly HttpClient _httpClient;
        private readonly DefaultBackChannelLogoutHttpClient _subject;

        public BackChannelLogoutHttpClientTests()
        {
            _logger = new Mock<ILogger<DefaultBackChannelLogoutHttpClient>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);
            
            _httpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpHandler.Object);
            _subject = new DefaultBackChannelLogoutHttpClient(_httpClient, _loggerFactory.Object);
        }

        [Fact]
        public async Task PostAsync_WhenSuccessful_ShouldLogDebug()
        {
            // Arrange
            var url = "https://test.com/logout";
            var payload = new Dictionary<string, string> { { "key", "value" } };

            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _subject.PostAsync(url, payload);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task PostAsync_WhenFailure_ShouldLogWarning()
        {
            // Arrange
            var url = "https://test.com/logout";
            var payload = new Dictionary<string, string> { { "key", "value" } };

            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act
            await _subject.PostAsync(url, payload);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task PostAsync_WhenException_ShouldLogError()
        {
            // Arrange
            var url = "https://test.com/logout";
            var payload = new Dictionary<string, string> { { "key", "value" } };

            _httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            await _subject.PostAsync(url, payload);

            // Assert
            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
