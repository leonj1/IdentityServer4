using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class IConsentMessageStoreTests
    {
        private class TestConsentMessageStore : IConsentMessageStore
        {
            private Message<ConsentResponse> _message;
            private string _id;

            public Task WriteAsync(string id, Message<ConsentResponse> message)
            {
                _id = id;
                _message = message;
                return Task.CompletedTask;
            }

            public Task<Message<ConsentResponse>> ReadAsync(string id)
            {
                if (id == _id)
                    return Task.FromResult(_message);
                return Task.FromResult<Message<ConsentResponse>>(null);
            }

            public Task DeleteAsync(string id)
            {
                if (id == _id)
                {
                    _message = null;
                    _id = null;
                }
                return Task.CompletedTask;
            }
        }

        private readonly IConsentMessageStore _subject;

        public IConsentMessageStoreTests()
        {
            _subject = new TestConsentMessageStore();
        }

        [Fact]
        public async Task WriteAsync_ShouldStoreMessage()
        {
            // Arrange
            var id = "test_id";
            var message = new Message<ConsentResponse>
            {
                Data = new ConsentResponse { RememberConsent = true },
                Created = DateTime.UtcNow
            };

            // Act
            await _subject.WriteAsync(id, message);
            var result = await _subject.ReadAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.RememberConsent.Should().BeTrue();
        }

        [Fact]
        public async Task ReadAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var result = await _subject.ReadAsync("non_existent_id");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMessage()
        {
            // Arrange
            var id = "test_id";
            var message = new Message<ConsentResponse>
            {
                Data = new ConsentResponse { RememberConsent = true },
                Created = DateTime.UtcNow
            };
            await _subject.WriteAsync(id, message);

            // Act
            await _subject.DeleteAsync(id);
            var result = await _subject.ReadAsync(id);

            // Assert
            result.Should().BeNull();
        }
    }
}
