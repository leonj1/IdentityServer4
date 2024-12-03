using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Services.Default
{
    public class DistributedDeviceFlowThrottlingServiceTests
    {
        private readonly Mock<IDistributedCache> _cache;
        private readonly Mock<ISystemClock> _clock;
        private readonly IdentityServerOptions _options;
        private readonly DistributedDeviceFlowThrottlingService _subject;
        private readonly DateTime _now;

        public DistributedDeviceFlowThrottlingServiceTests()
        {
            _cache = new Mock<IDistributedCache>();
            _clock = new Mock<ISystemClock>();
            _options = new IdentityServerOptions();
            _now = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            _clock.Setup(x => x.UtcNow).Returns(new DateTimeOffset(_now));

            _subject = new DistributedDeviceFlowThrottlingService(
                _cache.Object,
                _clock.Object,
                _options);
        }

        [Fact]
        public async Task ShouldSlowDown_WhenDeviceCodeIsNull_ExpectArgumentNullException()
        {
            // Arrange
            var details = new DeviceCode();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _subject.ShouldSlowDown(null, details));
        }

        [Fact]
        public async Task ShouldSlowDown_WhenFirstRequest_ExpectFalse()
        {
            // Arrange
            var deviceCode = "device_code";
            var details = new DeviceCode { Lifetime = 300 };
            _cache.Setup(x => x.GetStringAsync($"devicecode_{deviceCode}", default))
                .ReturnsAsync((string)null);

            // Act
            var result = await _subject.ShouldSlowDown(deviceCode, details);

            // Assert
            result.Should().BeFalse();
            _cache.Verify(x => x.SetStringAsync(
                $"devicecode_{deviceCode}",
                _now.ToString("O"),
                It.Is<DistributedCacheEntryOptions>(o => 
                    o.AbsoluteExpiration == DateTimeOffset.FromUnixTimeSeconds(_now.AddSeconds(details.Lifetime).Ticks / TimeSpan.TicksPerSecond)),
                default));
        }

        [Fact]
        public async Task ShouldSlowDown_WhenWithinInterval_ExpectTrue()
        {
            // Arrange
            var deviceCode = "device_code";
            var details = new DeviceCode { Lifetime = 300 };
            var lastSeen = _now.AddSeconds(-(_options.DeviceFlow.Interval - 1));
            
            _cache.Setup(x => x.GetStringAsync($"devicecode_{deviceCode}", default))
                .ReturnsAsync(lastSeen.ToString("O"));

            // Act
            var result = await _subject.ShouldSlowDown(deviceCode, details);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldSlowDown_WhenOutsideInterval_ExpectFalse()
        {
            // Arrange
            var deviceCode = "device_code";
            var details = new DeviceCode { Lifetime = 300 };
            var lastSeen = _now.AddSeconds(-(_options.DeviceFlow.Interval + 1));
            
            _cache.Setup(x => x.GetStringAsync($"devicecode_{deviceCode}", default))
                .ReturnsAsync(lastSeen.ToString("O"));

            // Act
            var result = await _subject.ShouldSlowDown(deviceCode, details);

            // Assert
            result.Should().BeFalse();
        }
    }
}
