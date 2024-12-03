using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Stores
{
    public class ValidatingClientStoreTests
    {
        private readonly Mock<IClientStore> _mockInnerStore;
        private readonly Mock<IClientConfigurationValidator> _mockValidator;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ILogger<ValidatingClientStore<IClientStore>>> _mockLogger;
        private readonly ValidatingClientStore<IClientStore> _subject;
        private readonly Client _testClient;

        public ValidatingClientStoreTests()
        {
            _mockInnerStore = new Mock<IClientStore>();
            _mockValidator = new Mock<IClientConfigurationValidator>();
            _mockEventService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<ValidatingClientStore<IClientStore>>>();
            
            _subject = new ValidatingClientStore<IClientStore>(
                _mockInnerStore.Object,
                _mockValidator.Object,
                _mockEventService.Object,
                _mockLogger.Object);

            _testClient = new Client
            {
                ClientId = "test_client"
            };
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientNotFound_ReturnsNull()
        {
            // Arrange
            _mockInnerStore.Setup(x => x.FindClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Client)null);

            // Act
            var result = await _subject.FindClientByIdAsync("not_found");

            // Assert
            result.Should().BeNull();
            _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ClientConfigurationValidationContext>()), Times.Never);
            _mockEventService.Verify(x => x.RaiseAsync(It.IsAny<InvalidClientConfigurationEvent>()), Times.Never);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenValidationSucceeds_ReturnsClient()
        {
            // Arrange
            _mockInnerStore.Setup(x => x.FindClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_testClient);
            
            _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ClientConfigurationValidationContext>()))
                .Callback<ClientConfigurationValidationContext>(context => context.IsValid = true);

            // Act
            var result = await _subject.FindClientByIdAsync("test_client");

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(_testClient);
            _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ClientConfigurationValidationContext>()), Times.Once);
            _mockEventService.Verify(x => x.RaiseAsync(It.IsAny<InvalidClientConfigurationEvent>()), Times.Never);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenValidationFails_ReturnsNull()
        {
            // Arrange
            const string errorMessage = "Invalid configuration";
            
            _mockInnerStore.Setup(x => x.FindClientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_testClient);
            
            _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ClientConfigurationValidationContext>()))
                .Callback<ClientConfigurationValidationContext>(context => 
                {
                    context.IsValid = false;
                    context.ErrorMessage = errorMessage;
                });

            // Act
            var result = await _subject.FindClientByIdAsync("test_client");

            // Assert
            result.Should().BeNull();
            _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ClientConfigurationValidationContext>()), Times.Once);
            _mockEventService.Verify(x => x.RaiseAsync(It.Is<InvalidClientConfigurationEvent>(e => 
                e.ClientId == _testClient.ClientId && 
                e.ErrorMessage == errorMessage)), Times.Once);
        }
    }
}
