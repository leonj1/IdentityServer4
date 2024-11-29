using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultDeviceFlowInteractionServiceTests
    {
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IUserSession> _mockUserSession;
        private readonly Mock<IDeviceFlowCodeService> _mockDeviceFlowCodeService;
        private readonly Mock<IResourceStore> _mockResourceStore;
        private readonly Mock<IScopeParser> _mockScopeParser;
        private readonly Mock<ILogger<DefaultDeviceFlowInteractionService>> _mockLogger;
        private readonly DefaultDeviceFlowInteractionService _service;

        public DefaultDeviceFlowInteractionServiceTests()
        {
            _mockClientStore = new Mock<IClientStore>();
            _mockUserSession = new Mock<IUserSession>();
            _mockDeviceFlowCodeService = new Mock<IDeviceFlowCodeService>();
            _mockResourceStore = new Mock<IResourceStore>();
            _mockScopeParser = new Mock<IScopeParser>();
            _mockLogger = new Mock<ILogger<DefaultDeviceFlowInteractionService>>();

            _service = new DefaultDeviceFlowInteractionService(
                _mockClientStore.Object,
                _mockUserSession.Object,
                _mockDeviceFlowCodeService.Object,
                _mockResourceStore.Object,
                _mockScopeParser.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAuthorizationContextAsync_WhenUserCodeNotFound_ReturnsNull()
        {
            // Arrange
            const string userCode = "invalid_code";
            _mockDeviceFlowCodeService.Setup(x => x.FindByUserCodeAsync(userCode))
                .ReturnsAsync((DeviceCode)null);

            // Act
            var result = await _service.GetAuthorizationContextAsync(userCode);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAuthorizationContextAsync_WhenClientNotFound_ReturnsNull()
        {
            // Arrange
            const string userCode = "valid_code";
            var deviceCode = new DeviceCode { ClientId = "client_id" };
            
            _mockDeviceFlowCodeService.Setup(x => x.FindByUserCodeAsync(userCode))
                .ReturnsAsync(deviceCode);
            _mockClientStore.Setup(x => x.FindClientByIdAsync(deviceCode.ClientId))
                .ReturnsAsync((Client)null);

            // Act
            var result = await _service.GetAuthorizationContextAsync(userCode);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleRequestAsync_WhenUserCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string userCode = null;
            var consent = new ConsentResponse();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _service.HandleRequestAsync(userCode, consent));
        }

        [Fact]
        public async Task HandleRequestAsync_WhenConsentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            const string userCode = "valid_code";
            ConsentResponse consent = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _service.HandleRequestAsync(userCode, consent));
        }

        [Fact]
        public async Task HandleRequestAsync_WhenDeviceCodeNotFound_ReturnsFailureResult()
        {
            // Arrange
            const string userCode = "invalid_code";
            var consent = new ConsentResponse();

            _mockDeviceFlowCodeService.Setup(x => x.FindByUserCodeAsync(userCode))
                .ReturnsAsync((DeviceCode)null);

            // Act
            var result = await _service.HandleRequestAsync(userCode, consent);

            // Assert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("Invalid user code");
        }

        [Fact]
        public async Task HandleRequestAsync_WhenSuccessful_UpdatesDeviceCode()
        {
            // Arrange
            const string userCode = "valid_code";
            var consent = new ConsentResponse 
            { 
                Description = "test description",
                RememberConsent = true,
                ScopesValuesConsented = new string[] { "scope1", "scope2" }
            };
            
            var deviceCode = new DeviceCode { ClientId = "client_id" };
            var client = new Client();
            var subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("sub", "123") }));
            const string sessionId = "session_id";

            _mockDeviceFlowCodeService.Setup(x => x.FindByUserCodeAsync(userCode))
                .ReturnsAsync(deviceCode);
            _mockClientStore.Setup(x => x.FindClientByIdAsync(deviceCode.ClientId))
                .ReturnsAsync(client);
            _mockUserSession.Setup(x => x.GetUserAsync())
                .ReturnsAsync(subject);
            _mockUserSession.Setup(x => x.GetSessionIdAsync())
                .ReturnsAsync(sessionId);

            // Act
            var result = await _service.HandleRequestAsync(userCode, consent);

            // Assert
            result.IsError.Should().BeFalse();
            _mockDeviceFlowCodeService.Verify(x => x.UpdateByUserCodeAsync(userCode, It.Is<DeviceCode>(d => 
                d.IsAuthorized &&
                d.Subject == subject &&
                d.SessionId == sessionId &&
                d.Description == consent.Description &&
                d.AuthorizedScopes == consent.ScopesValuesConsented
            )), Times.Once);
        }
    }
}
