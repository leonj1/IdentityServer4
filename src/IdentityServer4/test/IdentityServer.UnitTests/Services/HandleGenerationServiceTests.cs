using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class HandleGenerationServiceTests
    {
        private readonly IHandleGenerationService _subject;

        public HandleGenerationServiceTests()
        {
            _subject = new DefaultHandleGenerationService();
        }

        [Fact]
        public async Task GenerateAsync_ShouldReturnStringOfRequestedLength()
        {
            // Act
            var result = await _subject.GenerateAsync(42);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Length.Should().Be(42);
        }

        [Fact]
        public async Task GenerateAsync_WithDefaultLength_ShouldReturn32Characters()
        {
            // Act
            var result = await _subject.GenerateAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Length.Should().Be(32);
        }

        [Fact]
        public async Task GenerateAsync_ShouldReturnDifferentValuesOnSubsequentCalls()
        {
            // Act
            var result1 = await _subject.GenerateAsync();
            var result2 = await _subject.GenerateAsync();

            // Assert
            result1.Should().NotBe(result2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GenerateAsync_WithInvalidLength_ShouldThrowArgumentException(int length)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _subject.GenerateAsync(length));
        }
    }
}
