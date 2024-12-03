using IdentityServer4.Models;
using FluentAssertions;
using Xunit;
using System;
using System.Text;

namespace IdentityServer.UnitTests.Extensions
{
    public class HashExtensionsTests
    {
        [Fact]
        public void Sha256_WithValidString_ShouldReturnCorrectHash()
        {
            // Arrange
            var input = "test";
            var expected = "n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Sha256_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Sha256_WithNullString_ShouldReturnEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Sha256_WithByteArray_ShouldReturnCorrectHash()
        {
            // Arrange
            var input = Encoding.UTF8.GetBytes("test");
            var expected = Convert.FromBase64String("n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=");

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Sha256_WithNullByteArray_ShouldReturnNull()
        {
            // Arrange
            byte[] input = null;

            // Act
            var result = input.Sha256();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Sha512_WithValidString_ShouldReturnCorrectHash()
        {
            // Arrange
            var input = "test";
            var expected = "7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==";

            // Act
            var result = input.Sha512();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Sha512_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = input.Sha512();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Sha512_WithNullString_ShouldReturnEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.Sha512();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
