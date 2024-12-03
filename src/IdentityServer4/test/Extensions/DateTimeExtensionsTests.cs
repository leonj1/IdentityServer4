using System;
using FluentAssertions;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void HasExceeded_WhenTimeExceeded_ShouldReturnTrue()
        {
            // Arrange
            var now = new DateTime(2024, 1, 1, 12, 0, 0);
            var creationTime = now.AddSeconds(-11);
            var seconds = 10;

            // Act
            var result = creationTime.HasExceeded(seconds, now);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasExceeded_WhenTimeNotExceeded_ShouldReturnFalse()
        {
            // Arrange
            var now = new DateTime(2024, 1, 1, 12, 0, 0);
            var creationTime = now.AddSeconds(-5);
            var seconds = 10;

            // Act
            var result = creationTime.HasExceeded(seconds, now);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetLifetimeInSeconds_ShouldReturnCorrectDifference()
        {
            // Arrange
            var now = new DateTime(2024, 1, 1, 12, 0, 0);
            var creationTime = now.AddSeconds(-30);

            // Act
            var result = creationTime.GetLifetimeInSeconds(now);

            // Assert
            result.Should().Be(30);
        }

        [Fact]
        public void HasExpired_WithNullableDateTime_WhenNull_ShouldReturnFalse()
        {
            // Arrange
            DateTime? expirationTime = null;
            var now = new DateTime(2024, 1, 1, 12, 0, 0);

            // Act
            var result = expirationTime.HasExpired(now);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasExpired_WithNullableDateTime_WhenExpired_ShouldReturnTrue()
        {
            // Arrange
            DateTime? expirationTime = new DateTime(2024, 1, 1, 11, 0, 0);
            var now = new DateTime(2024, 1, 1, 12, 0, 0);

            // Act
            var result = expirationTime.HasExpired(now);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasExpired_WithDateTime_WhenExpired_ShouldReturnTrue()
        {
            // Arrange
            var expirationTime = new DateTime(2024, 1, 1, 11, 0, 0);
            var now = new DateTime(2024, 1, 1, 12, 0, 0);

            // Act
            var result = expirationTime.HasExpired(now);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasExpired_WithDateTime_WhenNotExpired_ShouldReturnFalse()
        {
            // Arrange
            var expirationTime = new DateTime(2024, 1, 1, 13, 0, 0);
            var now = new DateTime(2024, 1, 1, 12, 0, 0);

            // Act
            var result = expirationTime.HasExpired(now);

            // Assert
            result.Should().BeFalse();
        }
    }
}
