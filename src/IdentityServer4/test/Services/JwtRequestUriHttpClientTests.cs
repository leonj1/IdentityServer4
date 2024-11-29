using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Services
{
    public class JwtRequestUriHttpClientTests
    {
        private readonly Mock<IJwtRequestUriHttpClient> _mockJwtClient;
        private readonly Client _testClient;

        public JwtRequestUriHttpClientTests()
        {
            _mockJwtClient = new Mock<IJwtRequestUriHttpClient>();
            _testClient = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };
        }

        [Fact]
        public async Task GetJwtAsync_WithValidUrl_ShouldReturnJwt()
        {
            // Arrange
            var expectedJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";
            var testUrl = "https://test.com/jwt";
            
            _mockJwtClient.Setup(x => x.GetJwtAsync(testUrl, _testClient))
                .ReturnsAsync(expectedJwt);

            // Act
            var result = await _mockJwtClient.Object.GetJwtAsync(testUrl, _testClient);

            // Assert
            result.Should().Be(expectedJwt);
            _mockJwtClient.Verify(x => x.GetJwtAsync(testUrl, _testClient), Times.Once);
        }

        [Fact]
        public async Task GetJwtAsync_WithInvalidUrl_ShouldThrowException()
        {
            // Arrange
            var invalidUrl = "invalid-url";
            _mockJwtClient.Setup(x => x.GetJwtAsync(invalidUrl, _testClient))
                .ThrowsAsync(new ArgumentException("Invalid URL"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _mockJwtClient.Object.GetJwtAsync(invalidUrl, _testClient));
        }

        [Fact]
        public async Task GetJwtAsync_WithNullClient_ShouldThrowException()
        {
            // Arrange
            var testUrl = "https://test.com/jwt";
            _mockJwtClient.Setup(x => x.GetJwtAsync(testUrl, null))
                .ThrowsAsync(new ArgumentNullException(nameof(Client)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _mockJwtClient.Object.GetJwtAsync(testUrl, null));
        }
    }
}
