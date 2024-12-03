using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using Xunit;

namespace IdentityServer.UnitTests.Events
{
    public class EventTests
    {
        private class TestEvent : Event
        {
            public TestEvent(string name, EventTypes type, int id, string message = null) 
                : base("TestCategory", name, type, id, message)
            {
            }
        }

        [Fact]
        public void Constructor_ShouldSetProperties()
        {
            // Arrange & Act
            var evt = new TestEvent("TestName", EventTypes.Success, 1234, "Test Message");

            // Assert
            evt.Category.Should().Be("TestCategory");
            evt.Name.Should().Be("TestName");
            evt.EventType.Should().Be(EventTypes.Success);
            evt.Id.Should().Be(1234);
            evt.Message.Should().Be("Test Message");
        }

        [Fact]
        public void Constructor_WithNullCategory_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new TestEvent(null, EventTypes.Success, 1);
            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("category");
        }

        [Fact]
        public void Constructor_WithNullName_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new TestEvent(null, EventTypes.Success, 1);
            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("name");
        }

        [Fact]
        public void Obfuscate_ShouldMaskTokenExceptLastFourChars()
        {
            // Arrange
            var testEvent = new TestEvent("TestName", EventTypes.Success, 1);
            var token = "SecretToken12345";

            // Act
            var result = typeof(Event).GetMethod("Obfuscate", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { token }) as string;

            // Assert
            result.Should().Be("****2345");
        }

        [Fact]
        public void Obfuscate_WithShortToken_ShouldReturnAsterisks()
        {
            // Arrange
            var testEvent = new TestEvent("TestName", EventTypes.Success, 1);
            var token = "123";

            // Act
            var result = typeof(Event).GetMethod("Obfuscate", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { token }) as string;

            // Assert
            result.Should().Be("********");
        }

        [Fact]
        public async Task PrepareAsync_ShouldReturnCompletedTask()
        {
            // Arrange
            var testEvent = new TestEvent("TestName", EventTypes.Success, 1);

            // Act
            await testEvent.PrepareAsync();

            // Assert
            // No exception means success
        }

        [Fact]
        public void ToString_ShouldReturnSerializedEvent()
        {
            // Arrange
            var testEvent = new TestEvent("TestName", EventTypes.Success, 1, "Test Message");

            // Act
            var result = testEvent.ToString();

            // Assert
            result.Should().Contain("TestName");
            result.Should().Contain("Test Message");
            result.Should().Contain("TestCategory");
        }
    }
}
