using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class TestEventServiceTests
    {
        private TestEventService _sut;

        public TestEventServiceTests()
        {
            _sut = new TestEventService();
        }

        [Fact]
        public async Task RaiseAsync_ShouldStoreEvent()
        {
            // Arrange
            var testEvent = new TestEvent();

            // Act
            await _sut.RaiseAsync(testEvent);

            // Assert
            var raisedEvent = _sut.AssertEventWasRaised<TestEvent>();
            raisedEvent.Should().Be(testEvent);
        }

        [Fact]
        public void AssertEventWasRaised_WhenEventNotRaised_ShouldThrow()
        {
            // Act & Assert
            Action act = () => _sut.AssertEventWasRaised<TestEvent>();
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void CanRaiseEventType_ShouldAlwaysReturnTrue()
        {
            // Act
            var result = _sut.CanRaiseEventType(EventTypes.Success);

            // Assert
            result.Should().BeTrue();
        }

        private class TestEvent : Event
        {
            public TestEvent() : base(EventCategories.Authentication,
                "Test",
                EventTypes.Success,
                EventIds.UserLogin)
            {
            }
        }
    }
}
