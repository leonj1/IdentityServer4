using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer.UnitTests.Common;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class MessageStoreTests
    {
        private readonly MockMessageStore<Message<string>> _subject;

        public MessageStoreTests()
        {
            _subject = new MockMessageStore<Message<string>>();
        }

        [Fact]
        public async Task WriteAsync_ShouldReturnValidId()
        {
            // Arrange
            var message = new Message<string>
            {
                Data = "test_data",
                Created = DateTime.UtcNow,
                ClientId = "test_client"
            };

            // Act
            var id = await _subject.WriteAsync(message);

            // Assert
            id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ReadAsync_WithValidId_ShouldReturnMessage()
        {
            // Arrange
            var message = new Message<string>
            {
                Data = "test_data",
                Created = DateTime.UtcNow,
                ClientId = "test_client"
            };
            var id = await _subject.WriteAsync(message);

            // Act
            var result = await _subject.ReadAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(message.Data);
            result.ClientId.Should().Be(message.ClientId);
            result.Created.Should().BeCloseTo(message.Created, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ReadAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _subject.ReadAsync("invalid_id");

            // Assert
            result.Should().BeNull();
        }
    }
}
