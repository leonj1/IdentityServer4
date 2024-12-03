using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class MockReferenceTokenStoreTests
    {
        private readonly MockReferenceTokenStore _store;

        public MockReferenceTokenStoreTests()
        {
            _store = new MockReferenceTokenStore();
        }

        [Fact]
        public async Task GetReferenceTokenAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var handle = "test-handle";

            // Act
            Func<Task> act = async () => await _store.GetReferenceTokenAsync(handle);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var handle = "test-handle";

            // Act
            Func<Task> act = async () => await _store.RemoveReferenceTokenAsync(handle);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task RemoveReferenceTokensAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var subjectId = "test-subject";
            var clientId = "test-client";

            // Act
            Func<Task> act = async () => await _store.RemoveReferenceTokensAsync(subjectId, clientId);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task StoreReferenceTokenAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var token = new Token();

            // Act
            Func<Task> act = async () => await _store.StoreReferenceTokenAsync(token);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }
    }
}
