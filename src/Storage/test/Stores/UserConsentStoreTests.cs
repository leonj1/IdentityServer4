using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.UnitTests.Stores
{
    public class UserConsentStoreTests
    {
        private class TestUserConsentStore : IUserConsentStore
        {
            private Consent _storedConsent;

            public Task StoreUserConsentAsync(Consent consent)
            {
                _storedConsent = consent;
                return Task.CompletedTask;
            }

            public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
            {
                if (_storedConsent != null && 
                    _storedConsent.SubjectId == subjectId && 
                    _storedConsent.ClientId == clientId)
                {
                    return Task.FromResult(_storedConsent);
                }
                return Task.FromResult<Consent>(null);
            }

            public Task RemoveUserConsentAsync(string subjectId, string clientId)
            {
                if (_storedConsent != null &&
                    _storedConsent.SubjectId == subjectId &&
                    _storedConsent.ClientId == clientId)
                {
                    _storedConsent = null;
                }
                return Task.CompletedTask;
            }
        }

        private readonly IUserConsentStore _store;
        private readonly Consent _testConsent;

        public UserConsentStoreTests()
        {
            _store = new TestUserConsentStore();
            _testConsent = new Consent
            {
                SubjectId = "test_subject",
                ClientId = "test_client",
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddDays(1)
            };
        }

        [Fact]
        public async Task StoreUserConsent_ShouldStoreConsent()
        {
            await _store.StoreUserConsentAsync(_testConsent);
            
            var retrieved = await _store.GetUserConsentAsync(_testConsent.SubjectId, _testConsent.ClientId);
            Assert.NotNull(retrieved);
            Assert.Equal(_testConsent.SubjectId, retrieved.SubjectId);
            Assert.Equal(_testConsent.ClientId, retrieved.ClientId);
        }

        [Fact]
        public async Task GetUserConsent_WithNonExistentConsent_ShouldReturnNull()
        {
            var result = await _store.GetUserConsentAsync("nonexistent", "nonexistent");
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveUserConsent_ShouldRemoveConsent()
        {
            await _store.StoreUserConsentAsync(_testConsent);
            await _store.RemoveUserConsentAsync(_testConsent.SubjectId, _testConsent.ClientId);
            
            var result = await _store.GetUserConsentAsync(_testConsent.SubjectId, _testConsent.ClientId);
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveUserConsent_WithNonExistentConsent_ShouldNotThrow()
        {
            await _store.RemoveUserConsentAsync("nonexistent", "nonexistent");
            // Test passes if no exception is thrown
        }
    }
}
