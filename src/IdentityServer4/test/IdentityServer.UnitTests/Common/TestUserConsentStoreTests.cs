using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class TestUserConsentStoreTests
    {
        private TestUserConsentStore _store;
        private string _subjectId = "123";
        private string _clientId = "client";

        public TestUserConsentStoreTests()
        {
            _store = new TestUserConsentStore();
        }

        [Fact]
        public async Task StoreUserConsentAsync_WhenValidConsent_ShouldStoreAndRetrieveCorrectly()
        {
            // Arrange
            var consent = new Consent
            {
                ClientId = _clientId,
                SubjectId = _subjectId,
                Scopes = new[] { "scope1", "scope2" },
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddDays(1)
            };

            // Act
            await _store.StoreUserConsentAsync(consent);
            var retrieved = await _store.GetUserConsentAsync(_subjectId, _clientId);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved.ClientId.Should().Be(consent.ClientId);
            retrieved.SubjectId.Should().Be(consent.SubjectId);
            retrieved.Scopes.Should().BeEquivalentTo(consent.Scopes);
        }

        [Fact]
        public async Task GetUserConsentAsync_WhenConsentDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _store.GetUserConsentAsync("nonexistent", "nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveUserConsentAsync_WhenConsentExists_ShouldRemoveConsent()
        {
            // Arrange
            var consent = new Consent
            {
                ClientId = _clientId,
                SubjectId = _subjectId,
                Scopes = new[] { "scope1" }
            };
            await _store.StoreUserConsentAsync(consent);

            // Act
            await _store.RemoveUserConsentAsync(_subjectId, _clientId);
            var retrieved = await _store.GetUserConsentAsync(_subjectId, _clientId);

            // Assert
            retrieved.Should().BeNull();
        }
    }
}
