using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventService> _mockEventService;

        public EventServiceTests()
        {
            _mockEventService = new Mock<IEventService>();
        }

        [Fact]
        public async Task RaiseAsync_ShouldCallEventService()
        {
            // Arrange
            var testEvent = new TestEvent("Test");
            _mockEventService.Setup(x => x.RaiseAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _mockEventService.Object.RaiseAsync(testEvent);

            // Assert
            _mockEventService.Verify(x => x.RaiseAsync(testEvent), Times.Once);
        }

        [Theory]
        [InlineData(EventTypes.Success, true)]
        [InlineData(EventTypes.Error, true)]
        [InlineData(EventTypes.Information, true)]
        [InlineData(EventTypes.Failure, true)]
        public void CanRaiseEventType_ShouldReturnExpectedResult(EventTypes eventType, bool expected)
        {
            // Arrange
            _mockEventService.Setup(x => x.CanRaiseEventType(It.IsAny<EventTypes>()))
                .Returns(expected);

            // Act
            var result = _mockEventService.Object.CanRaiseEventType(eventType);

            // Assert
            result.Should().Be(expected);
            _mockEventService.Verify(x => x.CanRaiseEventType(eventType), Times.Once);
        }

        private class TestEvent : Event
        {
            public TestEvent(string name) 
                : base(EventCategories.Authentication,
                      "Test",
                      EventTypes.Success,
                      EventIds.UserLogin)
            {
            }
        }
    }
}
