using System.Collections.Generic;
using Xunit;
using IdentityServer4.Extensions;

namespace IdentityServer4.UnitTests.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_WithNullList_ReturnsTrue()
        {
            // Arrange
            IEnumerable<string> list = null;

            // Act
            var result = list.IsNullOrEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithEmptyList_ReturnsTrue()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list.IsNullOrEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithPopulatedList_ReturnsFalse()
        {
            // Arrange
            var list = new List<string> { "test" };

            // Act
            var result = list.IsNullOrEmpty();

            // Assert
            Assert.False(result);
        }
    }
}
