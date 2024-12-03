using System;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models.Messages
{
    public class MessageTests
    {
        [Fact]
        public void Constructor_WhenCalledWithData_ShouldSetProperties()
        {
            // Arrange
            var testData = "test data";
            var now = DateTime.UtcNow;

            // Act
            var message = new Message<string>(testData, now);

            // Assert
            message.Created.Should().Be(now.Ticks);
            message.Data.Should().Be(testData);
        }

        [Fact]
        public void Constructor_WhenCalledWithoutParameters_ShouldCreateEmptyMessage()
        {
            // Act
            var message = new Message<string>();

            // Assert
            message.Created.Should().Be(0);
            message.Data.Should().BeNull();
        }

        [Fact]
        public void InternalConstructor_WhenCalledWithData_ShouldSetPropertiesWithCurrentTime()
        {
            // Arrange
            var testData = "test data";
            var beforeCreate = DateTime.UtcNow;

            // Act
            var message = new Message<string>(testData);
            var afterCreate = DateTime.UtcNow;

            // Assert
            message.Created.Should().BeGreaterOrEqualTo(beforeCreate.Ticks);
            message.Created.Should().BeLessOrEqualTo(afterCreate.Ticks);
            message.Data.Should().Be(testData);
        }
    }
}
