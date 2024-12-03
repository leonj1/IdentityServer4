using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Stores
{
    public class ProtectedDataMessageStoreTests
    {
        private readonly IDataProtector _dataProtector;
        private readonly Mock<IDataProtectionProvider> _provider;
        private readonly Mock<ILogger<ProtectedDataMessageStore<Message>>> _logger;
        private readonly ProtectedDataMessageStore<Message> _subject;

        public ProtectedDataMessageStoreTests()
        {
            _provider = new Mock<IDataProtectionProvider>();
            _dataProtector = new MockDataProtector();
            _provider.Setup(x => x.CreateProtector(It.IsAny<string>()))
                .Returns(_dataProtector);
            _logger = new Mock<ILogger<ProtectedDataMessageStore<Message>>>();
            _subject = new ProtectedDataMessageStore<Message>(_provider.Object, _logger.Object);
        }

        [Fact]
        public async Task WriteAsync_WhenGivenMessage_ShouldReturnProtectedData()
        {
            // Arrange
            var message = new Message<Message>
            {
                Created = DateTime.UtcNow,
                Data = new Message("test")
            };

            // Act
            var result = await _subject.WriteAsync(message);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ReadAsync_WhenGivenValidProtectedData_ShouldReturnOriginalMessage()
        {
            // Arrange
            var message = new Message<Message>
            {
                Created = DateTime.UtcNow,
                Data = new Message("test")
            };
            var protectedData = await _subject.WriteAsync(message);

            // Act
            var result = await _subject.ReadAsync(protectedData);

            // Assert
            result.Should().NotBeNull();
            result.Data.Data.Should().Be("test");
            result.Created.Should().BeCloseTo(message.Created, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ReadAsync_WhenGivenInvalidData_ShouldReturnNull()
        {
            // Act
            var result = await _subject.ReadAsync("invalid_data");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ReadAsync_WhenGivenEmptyString_ShouldReturnNull()
        {
            // Act
            var result = await _subject.ReadAsync(string.Empty);

            // Assert
            result.Should().BeNull();
        }
    }

    // Helper mock implementation of IDataProtector
    public class MockDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return this;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
