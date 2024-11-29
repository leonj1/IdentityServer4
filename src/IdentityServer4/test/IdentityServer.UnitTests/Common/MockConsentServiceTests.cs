using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Xunit;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.UnitTests.Common
{
    public class MockConsentServiceTests
    {
        private MockConsentService _mockConsentService;
        private ClaimsPrincipal _subject;
        private Client _client;
        private List<ParsedScopeValue> _parsedScopes;

        public MockConsentServiceTests()
        {
            _mockConsentService = new MockConsentService();
            _subject = new ClaimsPrincipal(new ClaimsIdentity());
            _client = new Client();
            _parsedScopes = new List<ParsedScopeValue> 
            { 
                new ParsedScopeValue("scope1"),
                new ParsedScopeValue("scope2")
            };
        }

        [Fact]
        public async void RequiresConsentAsync_ShouldReturnConfiguredResult()
        {
            // Arrange
            _mockConsentService.RequiresConsentResult = true;

            // Act
            var result = await _mockConsentService.RequiresConsentAsync(_subject, _client, _parsedScopes);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async void UpdateConsentAsync_ShouldStoreProvidedValues()
        {
            // Act
            await _mockConsentService.UpdateConsentAsync(_subject, _client, _parsedScopes);

            // Assert
            Assert.Same(_subject, _mockConsentService.ConsentSubject);
            Assert.Same(_client, _mockConsentService.ConsentClient);
            Assert.Equal(_parsedScopes.Select(x => x.RawValue), _mockConsentService.ConsentScopes);
        }
    }
}
