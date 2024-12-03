using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultDeviceFlowCodeServiceTests
    {
        private readonly Mock<IDeviceFlowStore> _mockStore;
        private readonly Mock<IHandleGenerationService> _mockHandleGenerationService;
        private readonly IDeviceFlowCodeService _service;
        private readonly DeviceCode _testDeviceCode;

        public DefaultDeviceFlowCodeServiceTests()
        {
            _mockStore = new Mock<IDeviceFlowStore>();
            _mockHandleGenerationService = new Mock<IHandleGenerationService>();
            _service = new DefaultDeviceFlowCodeService(_mockStore.Object, _mockHandleGenerationService.Object);
            
            _testDeviceCode = new DeviceCode
            {
                ClientId = "client1",
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_ShouldStoreAndReturnDeviceCode()
        {
            // Arrange
            var userCode = "userCode123";
            var generatedDeviceCode = "deviceCode123";
            
            _mockHandleGenerationService.Setup(x => x.GenerateAsync())
                .ReturnsAsync(generatedDeviceCode);

            // Act
            var result = await _service.StoreDeviceAuthorizationAsync(userCode, _testDeviceCode);

            // Assert
            result.Should().Be(generatedDeviceCode);
            _mockStore.Verify(x => x.StoreDeviceAuthorizationAsync(
                generatedDeviceCode.Sha256(),
                userCode.Sha256(), 
                _testDeviceCode), 
                Times.Once);
        }

        [Fact]
        public async Task FindByUserCodeAsync_ShouldReturnDeviceCode()
        {
            // Arrange
            var userCode = "userCode123";
            _mockStore.Setup(x => x.FindByUserCodeAsync(userCode.Sha256()))
                .ReturnsAsync(_testDeviceCode);

            // Act
            var result = await _service.FindByUserCodeAsync(userCode);

            // Assert
            result.Should().BeEquivalentTo(_testDeviceCode);
            _mockStore.Verify(x => x.FindByUserCodeAsync(userCode.Sha256()), Times.Once);
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_ShouldReturnDeviceCode()
        {
            // Arrange
            var deviceCode = "deviceCode123";
            _mockStore.Setup(x => x.FindByDeviceCodeAsync(deviceCode.Sha256()))
                .ReturnsAsync(_testDeviceCode);

            // Act
            var result = await _service.FindByDeviceCodeAsync(deviceCode);

            // Assert
            result.Should().BeEquivalentTo(_testDeviceCode);
            _mockStore.Verify(x => x.FindByDeviceCodeAsync(deviceCode.Sha256()), Times.Once);
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_ShouldCallStore()
        {
            // Arrange
            var userCode = "userCode123";

            // Act
            await _service.UpdateByUserCodeAsync(userCode, _testDeviceCode);

            // Assert
            _mockStore.Verify(x => x.UpdateByUserCodeAsync(userCode.Sha256(), _testDeviceCode), Times.Once);
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_ShouldCallStore()
        {
            // Arrange
            var deviceCode = "deviceCode123";

            // Act
            await _service.RemoveByDeviceCodeAsync(deviceCode);

            // Assert
            _mockStore.Verify(x => x.RemoveByDeviceCodeAsync(deviceCode.Sha256()), Times.Once);
        }
    }
}
