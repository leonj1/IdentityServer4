using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class EventSinkTests
    {
        private readonly Mock<IEventSink> _mockEventSink;

        public EventSinkTests()
        {
            _mockEventSink = new Mock<IEventSink>();
        }

        [Fact]
        public async Task PersistAsync_ShouldAcceptEvent()
        {
            // Arrange
            var testEvent = new Event("Test Event");
            _mockEventSink.Setup(x => x.PersistAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _mockEventSink.Object.PersistAsync(testEvent);

            // Assert
            _mockEventSink.Verify(x => x.PersistAsync(testEvent), Times.Once);
        }

        [Fact]
        public async Task PersistAsync_ShouldHandleNullEvent()
        {
            // Arrange
            Event nullEvent = null;
            _mockEventSink.Setup(x => x.PersistAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _mockEventSink.Object.PersistAsync(nullEvent));
        }
    }
}
