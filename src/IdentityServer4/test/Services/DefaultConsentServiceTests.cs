using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer4.UnitTests.Services
{
    public class DefaultConsentServiceTests
    {
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IUserConsentStore> _mockUserConsentStore;
        private readonly Mock<ILogger<DefaultConsentService>> _mockLogger;
        private readonly DefaultConsentService _consentService;
        private readonly ClaimsPrincipal _user;
        private readonly Client _client;

        public DefaultConsentServiceTests()
        {
            _mockClock = new Mock<ISystemClock>();
            _mockUserConsentStore = new Mock<IUserConsentStore>();
            _mockLogger = new Mock<ILogger<DefaultConsentService>>();
            
            _consentService = new DefaultConsentService(
                _mockClock.Object,
                _mockUserConsentStore.Object,
                _mockLogger.Object);

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            _client = new Client
            {
                ClientId = "client",
                RequireConsent = true,
                AllowRememberConsent = true
            };

            _mockClock.Setup(x => x.UtcNow)
                .Returns(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public async Task RequiresConsent_WhenClientDoesNotRequireConsent_ReturnsFalse()
        {
            // Arrange
            _client.RequireConsent = false;
            var scopes = new[] { new ParsedScopeValue("scope1") };

            // Act
            var result = await _consentService.RequiresConsentAsync(_user, _client, scopes);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsent_WhenNoScopes_ReturnsFalse()
        {
            // Arrange
            var scopes = Array.Empty<ParsedScopeValue>();

            // Act
            var result = await _consentService.RequiresConsentAsync(_user, _client, scopes);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsent_WhenClientDoesNotAllowRememberConsent_ReturnsTrue()
        {
            // Arrange
            _client.AllowRememberConsent = false;
            var scopes = new[] { new ParsedScopeValue("scope1") };

            // Act
            var result = await _consentService.RequiresConsentAsync(_user, _client, scopes);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsent_WhenValidConsentExists_ReturnsFalse()
        {
            // Arrange
            var scopes = new[] { new ParsedScopeValue("scope1") };
            var consent = new Consent
            {
                ClientId = _client.ClientId,
                SubjectId = "123",
                Scopes = new[] { "scope1" },
                CreationTime = _mockClock.Object.UtcNow.UtcDateTime,
                Expiration = _mockClock.Object.UtcNow.UtcDateTime.AddDays(1)
            };

            _mockUserConsentStore.Setup(x => x.GetUserConsentAsync("123", _client.ClientId))
                .ReturnsAsync(consent);

            // Act
            var result = await _consentService.RequiresConsentAsync(_user, _client, scopes);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateConsent_StoresConsentCorrectly()
        {
            // Arrange
            var scopes = new[] { new ParsedScopeValue("scope1") };
            Consent storedConsent = null;

            _mockUserConsentStore.Setup(x => x.StoreUserConsentAsync(It.IsAny<Consent>()))
                .Callback<Consent>(c => storedConsent = c)
                .Returns(Task.CompletedTask);

            // Act
            await _consentService.UpdateConsentAsync(_user, _client, scopes);

            // Assert
            storedConsent.Should().NotBeNull();
            storedConsent.ClientId.Should().Be(_client.ClientId);
            storedConsent.SubjectId.Should().Be("123");
            storedConsent.Scopes.Should().Contain("scope1");
            storedConsent.CreationTime.Should().Be(_mockClock.Object.UtcNow.UtcDateTime);
        }

        [Fact]
        public async Task UpdateConsent_WhenNoScopes_RemovesConsent()
        {
            // Arrange
            string removedSubjectId = null;
            string removedClientId = null;

            _mockUserConsentStore.Setup(x => x.RemoveUserConsentAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((s, c) => {
                    removedSubjectId = s;
                    removedClientId = c;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _consentService.UpdateConsentAsync(_user, _client, null);

            // Assert
            removedSubjectId.Should().Be("123");
            removedClientId.Should().Be(_client.ClientId);
        }
    }
}
