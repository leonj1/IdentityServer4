using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class ConsentServiceTests
    {
        private readonly ClaimsPrincipal _subject;
        private readonly Client _client;
        private readonly IEnumerable<ParsedScopeValue> _parsedScopes;

        public ConsentServiceTests()
        {
            _subject = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("sub", "123")
            }));

            _client = new Client
            {
                ClientId = "test_client",
                RequireConsent = true
            };

            _parsedScopes = new[]
            {
                new ParsedScopeValue("scope1"),
                new ParsedScopeValue("scope2")
            };
        }

        [Fact]
        public async Task RequiresConsentAsync_WhenClientRequiresConsent_ShouldReturnTrue()
        {
            // Arrange
            var consentService = new DefaultConsentService();

            // Act
            var result = await consentService.RequiresConsentAsync(_subject, _client, _parsedScopes);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_WhenClientDoesNotRequireConsent_ShouldReturnFalse()
        {
            // Arrange
            var consentService = new DefaultConsentService();
            var client = new Client
            {
                ClientId = "test_client",
                RequireConsent = false
            };

            // Act
            var result = await consentService.RequiresConsentAsync(_subject, client, _parsedScopes);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateConsentAsync_ShouldNotThrowException()
        {
            // Arrange
            var consentService = new DefaultConsentService();

            // Act & Assert
            await consentService.Invoking(x => 
                x.UpdateConsentAsync(_subject, _client, _parsedScopes))
                .Should().NotThrowAsync();
        }
    }
}
