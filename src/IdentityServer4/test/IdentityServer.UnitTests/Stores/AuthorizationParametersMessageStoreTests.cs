using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class AuthorizationParametersMessageStoreTests
    {
        private readonly MockAuthorizationParametersMessageStore _store;

        public AuthorizationParametersMessageStoreTests()
        {
            _store = new MockAuthorizationParametersMessageStore();
        }

        [Fact]
        public async Task WriteAsync_ShouldStoreMessage()
        {
            // Arrange
            var message = new Message<IDictionary<string, string[]>>(
                new Dictionary<string, string[]> { { "test", new[] { "value" } } },
                DateTime.UtcNow);

            // Act
            var id = await _store.WriteAsync(message);

            // Assert
            id.Should().NotBeNullOrEmpty();
            var retrieved = await _store.ReadAsync(id);
            retrieved.Should().NotBeNull();
            retrieved.Data.Should().ContainKey("test");
            retrieved.Data["test"].Should().BeEquivalentTo(new[] { "value" });
        }

        [Fact]
        public async Task ReadAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _store.ReadAsync("invalid_id");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMessage()
        {
            // Arrange
            var message = new Message<IDictionary<string, string[]>>(
                new Dictionary<string, string[]> { { "test", new[] { "value" } } },
                DateTime.UtcNow);
            var id = await _store.WriteAsync(message);

            // Act
            await _store.DeleteAsync(id);

            // Assert
            var result = await _store.ReadAsync(id);
            result.Should().BeNull();
        }
    }

    public class MockAuthorizationParametersMessageStore : IAuthorizationParametersMessageStore
    {
        private readonly Dictionary<string, Message<IDictionary<string, string[]>>> _store = 
            new Dictionary<string, Message<IDictionary<string, string[]>>>();

        public Task<string> WriteAsync(Message<IDictionary<string, string[]>> message)
        {
            var id = Guid.NewGuid().ToString();
            _store[id] = message;
            return Task.FromResult(id);
        }

        public Task<Message<IDictionary<string, string[]>>> ReadAsync(string id)
        {
            _store.TryGetValue(id, out var message);
            return Task.FromResult(message);
        }

        public Task DeleteAsync(string id)
        {
            _store.Remove(id);
            return Task.CompletedTask;
        }
    }
}
