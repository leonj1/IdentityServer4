using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Xunit;
using IdentityServer4.Extensions;

namespace IdentityServer.UnitTests.Extensions
{
    public class IReadableStringCollectionExtensionsTests
    {
        [Fact]
        public void AsNameValueCollection_FromIEnumerable_ShouldConvertCorrectly()
        {
            // Arrange
            var collection = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("key1", new StringValues("value1")),
                new KeyValuePair<string, StringValues>("key2", new StringValues(new[] { "value2", "ignored" }))
            };

            // Act
            var result = collection.AsNameValueCollection();

            // Assert
            result.Should().BeOfType<NameValueCollection>();
            result["key1"].Should().Be("value1");
            result["key2"].Should().Be("value2");
        }

        [Fact]
        public void AsNameValueCollection_FromIDictionary_ShouldConvertCorrectly()
        {
            // Arrange
            var dictionary = new Dictionary<string, StringValues>
            {
                { "key1", new StringValues("value1") },
                { "key2", new StringValues(new[] { "value2", "ignored" }) }
            };

            // Act
            var result = dictionary.AsNameValueCollection();

            // Assert
            result.Should().BeOfType<NameValueCollection>();
            result["key1"].Should().Be("value1");
            result["key2"].Should().Be("value2");
        }
    }
}
