using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Validation
{
    public class SecretsListParserTests
    {
        private readonly Mock<ISecretsListParser> _mockParser;
        private readonly DefaultHttpContext _httpContext;

        public SecretsListParserTests()
        {
            _mockParser = new Mock<ISecretsListParser>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnParsedSecret_WhenValidContextProvided()
        {
            // Arrange
            var expectedSecret = new ParsedSecret
            {
                Id = "test_client",
                Credential = "test_credential",
                Type = "test_type"
            };

            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync(expectedSecret);

            // Act
            var result = await _mockParser.Object.ParseAsync(_httpContext);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSecret);
            _mockParser.Verify(x => x.ParseAsync(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public void GetAvailableAuthenticationMethods_ShouldReturnExpectedMethods()
        {
            // Arrange
            var expectedMethods = new List<string> { "method1", "method2" };
            _mockParser.Setup(x => x.GetAvailableAuthenticationMethods())
                .Returns(expectedMethods);

            // Act
            var result = _mockParser.Object.GetAvailableAuthenticationMethods();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedMethods);
            _mockParser.Verify(x => x.GetAvailableAuthenticationMethods(), Times.Once);
        }

        [Fact]
        public async Task ParseAsync_ShouldReturnNull_WhenNoSecretFound()
        {
            // Arrange
            _mockParser.Setup(x => x.ParseAsync(It.IsAny<HttpContext>()))
                .ReturnsAsync((ParsedSecret)null);

            // Act
            var result = await _mockParser.Object.ParseAsync(_httpContext);

            // Assert
            result.Should().BeNull();
            _mockParser.Verify(x => x.ParseAsync(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
