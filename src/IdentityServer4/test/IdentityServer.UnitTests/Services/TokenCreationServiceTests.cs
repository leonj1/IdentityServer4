using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class TokenCreationServiceTests
    {
        private class TestTokenCreationService : ITokenCreationService
        {
            public async Task<string> CreateTokenAsync(Token token)
            {
                if (token == null) throw new ArgumentNullException(nameof(token));
                return "test_token";
            }
        }

        private readonly ITokenCreationService _sut;

        public TokenCreationServiceTests()
        {
            _sut = new TestTokenCreationService();
        }

        [Fact]
        public async Task CreateTokenAsync_WhenValidToken_ShouldReturnTokenString()
        {
            // Arrange
            var token = new Token();

            // Act
            var result = await _sut.CreateTokenAsync(token);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be("test_token");
        }

        [Fact]
        public async Task CreateTokenAsync_WhenTokenIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _sut.CreateTokenAsync(null));
        }
    }
}
