using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IdentityServer4.UnitTests.Services
{
    public class DefaultEventServiceTests
    {
        private readonly Mock<IEventSink> _mockEventSink;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ISystemClock> _mockClock;
        private readonly IdentityServerOptions _options;
        private readonly DefaultEventService _eventService;
        private readonly HttpContext _httpContext;

        public DefaultEventServiceTests()
        {
            _mockEventSink = new Mock<IEventSink>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockClock = new Mock<ISystemClock>();
            _options = new IdentityServerOptions();

            _httpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);

            _eventService = new DefaultEventService(
                _options,
                _mockHttpContextAccessor.Object,
                _mockEventSink.Object,
                _mockClock.Object
            );
        }

        [Fact]
        public async Task RaiseAsync_WhenEventIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _eventService.RaiseAsync(null));
        }

        [Theory]
        [InlineData(EventTypes.Success, true)]
        [InlineData(EventTypes.Failure, true)]
        [InlineData(EventTypes.Information, true)]
        [InlineData(EventTypes.Error, true)]
        public void CanRaiseEventType_WhenEventTypeIsValid_ShouldReturnExpectedResult(EventTypes eventType, bool enableEvents)
        {
            // Arrange
            _options.Events.RaiseSuccessEvents = enableEvents;
            _options.Events.RaiseFailureEvents = enableEvents;
            _options.Events.RaiseInformationEvents = enableEvents;
            _options.Events.RaiseErrorEvents = enableEvents;

            // Act
            var result = _eventService.CanRaiseEventType(eventType);

            // Assert
            result.Should().Be(enableEvents);
        }

        [Fact]
        public void CanRaiseEventType_WhenEventTypeIsInvalid_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            const EventTypes invalidEventType = (EventTypes)999;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _eventService.CanRaiseEventType(invalidEventType));
        }

        [Fact]
        public async Task RaiseAsync_WhenEventCanBeRaised_ShouldPersistEvent()
        {
            // Arrange
            var testEvent = new TestEvent();
            _options.Events.RaiseSuccessEvents = true;

            // Act
            await _eventService.RaiseAsync(testEvent);

            // Assert
            _mockEventSink.Verify(x => x.PersistAsync(It.Is<Event>(e => 
                e == testEvent &&
                e.ActivityId == _httpContext.TraceIdentifier
            )), Times.Once);
        }

        [Fact]
        public async Task RaiseAsync_WhenEventCannotBeRaised_ShouldNotPersistEvent()
        {
            // Arrange
            var testEvent = new TestEvent();
            _options.Events.RaiseSuccessEvents = false;

            // Act
            await _eventService.RaiseAsync(testEvent);

            // Assert
            _mockEventSink.Verify(x => x.PersistAsync(It.IsAny<Event>()), Times.Never);
        }

        private class TestEvent : Event
        {
            public TestEvent() : base(EventTypes.Success, 1000, "Test event") { }
        }
    }
}
