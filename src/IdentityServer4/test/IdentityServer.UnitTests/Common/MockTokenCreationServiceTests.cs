using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockTokenCreationServiceTests
    {
        [Fact]
        public async Task CreateTokenAsync_ShouldReturnConfiguredToken()
        {
            // Arrange
            var expectedToken = "test_token";
            var service = new MockTokenCreationService
            {
                Token = expectedToken
            };
            var token = new Token();

            // Act
            var result = await service.CreateTokenAsync(token);

            // Assert
            result.Should().Be(expectedToken);
        }
    }
}
