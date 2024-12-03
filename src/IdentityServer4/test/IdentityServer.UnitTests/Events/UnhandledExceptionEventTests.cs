using System;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class UnhandledExceptionEventTests
    {
        [Fact]
        public void UnhandledExceptionEvent_should_contain_exception_data()
        {
            // Arrange
            var expectedMessage = "Test exception message";
            var expectedException = new Exception(expectedMessage);

            // Act
            var evt = new UnhandledExceptionEvent(expectedException);

            // Assert
            evt.Should().BeAssignableTo<Event>();
            evt.Category.Should().Be(EventCategories.Error);
            evt.Name.Should().Be("Unhandled Exception");
            evt.EventType.Should().Be(EventTypes.Error);
            evt.Id.Should().Be(EventIds.UnhandledException);
            evt.Message.Should().Be(expectedMessage);
            evt.Details.Should().Be(expectedException.ToString());
        }
    }
}
