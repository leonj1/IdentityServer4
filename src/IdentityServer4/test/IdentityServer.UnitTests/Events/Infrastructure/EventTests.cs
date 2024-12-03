using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events.Infrastructure
{
    public class EventTests
    {
        private class TestEvent : Event
        {
            public TestEvent() : base("TestCategory", "TestName", EventTypes.Success, 1, "Test Message")
            {
            }
        }

        [Fact]
        public void Constructor_ShouldSetBasicProperties()
        {
            // Arrange & Act
            var evt = new TestEvent();

            // Assert
            evt.Category.Should().Be("TestCategory");
            evt.Name.Should().Be("TestName");
            evt.EventType.Should().Be(EventTypes.Success);
            evt.Id.Should().Be(1);
            evt.Message.Should().Be("Test Message");
        }

        [Fact]
        public void Constructor_WithNullCategory_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new Event(null, "name", EventTypes.Success, 1);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("category");
        }

        [Fact]
        public void Constructor_WithNullName_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new Event("category", null, EventTypes.Success, 1);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("name");
        }

        [Fact]
        public async Task PrepareAsync_ShouldReturnCompletedTask()
        {
            // Arrange
            var evt = new TestEvent();

            // Act
            await evt.PrepareAsync();

            // Assert
            // No exception means success
        }

        [Theory]
        [InlineData("token123", "****123")]
        [InlineData("12", "****")]
        [InlineData("", "****")]
        [InlineData(null, "****")]
        public void Obfuscate_ShouldCorrectlyMaskValue(string input, string expected)
        {
            // Arrange
            var evt = new TestEvent();
            var method = typeof(Event).GetMethod("Obfuscate", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input });

            // Assert
            result.Should().Be(expected);
        }
    }
}
