using Microsoft.AspNetCore.Authentication;
using System;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockSystemClockTests
    {
        [Fact]
        public void UtcNow_ShouldReturnConfiguredTime()
        {
            // Arrange
            var expectedTime = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);
            var clock = new MockSystemClock { Now = expectedTime };

            // Act
            var actualTime = clock.UtcNow;

            // Assert
            Assert.Equal(expectedTime, actualTime);
        }

        [Fact]
        public void Now_ShouldBeSettable()
        {
            // Arrange
            var clock = new MockSystemClock();
            var newTime = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);

            // Act
            clock.Now = newTime;

            // Assert
            Assert.Equal(newTime, clock.Now);
            Assert.Equal(newTime, clock.UtcNow);
        }
    }
}
