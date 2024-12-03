using System;
using FluentAssertions;
using IdentityServer4.Extensions;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.UnitTests.Extensions
{
    public class PersistedGrantFilterExtensionsTests
    {
        [Fact]
        public void Validate_WhenFilterIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => ((PersistedGrantFilter)null).Validate();

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("filter");
        }

        [Fact]
        public void Validate_WhenAllFilterValuesAreEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var filter = new PersistedGrantFilter
            {
                ClientId = "",
                SessionId = "",
                SubjectId = "",
                Type = ""
            };

            // Act
            Action act = () => filter.Validate();

            // Assert
            act.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("filter")
                .And.Message.Should().Contain("No filter values set");
        }

        [Theory]
        [InlineData("clientId", "", "", "")]
        [InlineData("", "sessionId", "", "")]
        [InlineData("", "", "subjectId", "")]
        [InlineData("", "", "", "type")]
        public void Validate_WhenAtLeastOneFilterValueIsSet_ShouldNotThrow(
            string clientId, string sessionId, string subjectId, string type)
        {
            // Arrange
            var filter = new PersistedGrantFilter
            {
                ClientId = clientId,
                SessionId = sessionId,
                SubjectId = subjectId,
                Type = type
            };

            // Act
            Action act = () => filter.Validate();

            // Assert
            act.Should().NotThrow();
        }
    }
}
