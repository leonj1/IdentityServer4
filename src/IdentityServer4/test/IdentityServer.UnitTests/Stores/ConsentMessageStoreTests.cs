using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class ConsentMessageStoreTests
    {
        private readonly MessageCookie<ConsentResponse> _cookie;
        private readonly ConsentMessageStore _subject;

        public ConsentMessageStoreTests()
        {
            _cookie = new MessageCookie<ConsentResponse>();
            _subject = new ConsentMessageStore(_cookie);
        }

        [Fact]
        public async Task WriteAsync_ShouldStoreMessage()
        {
            // Arrange
            var id = "test_id";
            var message = new Message<ConsentResponse>(new ConsentResponse(), 60);

            // Act
            await _subject.WriteAsync(id, message);
            var result = await _subject.ReadAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(message.Data);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMessage()
        {
            // Arrange
            var id = "test_id";
            var message = new Message<ConsentResponse>(new ConsentResponse(), 60);
            await _subject.WriteAsync(id, message);

            // Act
            await _subject.DeleteAsync(id);
            var result = await _subject.ReadAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ReadAsync_WhenMessageDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _subject.ReadAsync("non_existent_id");

            // Assert
            result.Should().BeNull();
        }
    }
}
