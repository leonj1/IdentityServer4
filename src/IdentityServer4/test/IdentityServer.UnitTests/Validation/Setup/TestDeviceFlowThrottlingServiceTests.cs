using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestDeviceFlowThrottlingServiceTests
    {
        [Fact]
        public async Task When_Configured_To_Not_SlowDown_Should_Return_False()
        {
            // Arrange
            var service = new TestDeviceFlowThrottlingService(shouldSlownDown: false);
            var deviceCode = "device_code";
            var details = new DeviceCode();

            // Act
            var result = await service.ShouldSlowDown(deviceCode, details);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task When_Configured_To_SlowDown_Should_Return_True()
        {
            // Arrange
            var service = new TestDeviceFlowThrottlingService(shouldSlownDown: true);
            var deviceCode = "device_code";
            var details = new DeviceCode();

            // Act
            var result = await service.ShouldSlowDown(deviceCode, details);

            // Assert
            result.Should().BeTrue();
        }
    }
}
