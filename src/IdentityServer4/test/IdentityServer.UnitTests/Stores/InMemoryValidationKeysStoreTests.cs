using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemoryValidationKeysStoreTests
    {
        [Fact]
        public void Constructor_WhenKeysNull_ThrowsArgumentNullException()
        {
            Action act = () => new InMemoryValidationKeysStore(null);
            
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("keys");
        }

        [Fact]
        public async Task GetValidationKeys_WhenKeysProvided_ReturnsExpectedKeys()
        {
            // Arrange
            var expectedKeys = new[]
            {
                new SecurityKeyInfo { Key = "key1" },
                new SecurityKeyInfo { Key = "key2" }
            };
            var store = new InMemoryValidationKeysStore(expectedKeys);

            // Act
            var result = await store.GetValidationKeysAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedKeys);
        }

        [Fact]
        public async Task GetValidationKeys_WhenEmptyKeysProvided_ReturnsEmptyCollection()
        {
            // Arrange
            var store = new InMemoryValidationKeysStore(new List<SecurityKeyInfo>());

            // Act
            var result = await store.GetValidationKeysAsync();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
