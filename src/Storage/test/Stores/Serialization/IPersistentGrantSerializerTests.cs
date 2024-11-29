using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using IdentityServer4.Stores.Serialization;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class IPersistentGrantSerializerTests
    {
        private readonly IPersistentGrantSerializer _serializer;
        private readonly TestClass _testData;

        public IPersistentGrantSerializerTests()
        {
            _serializer = new PersistentGrantSerializer();
            _testData = new TestClass
            {
                Id = 123,
                Name = "Test Name",
                CreatedAt = new DateTime(2024, 1, 1),
                Items = new List<string> { "item1", "item2" }
            };
        }

        [Fact]
        public void Serialize_ShouldSerializeObjectToJson()
        {
            // Act
            var json = _serializer.Serialize(_testData);

            // Assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("Test Name");
            json.Should().Contain("123");
            json.Should().Contain("2024");
        }

        [Fact]
        public void Deserialize_ShouldDeserializeJsonToObject()
        {
            // Arrange
            var json = _serializer.Serialize(_testData);

            // Act
            var result = _serializer.Deserialize<TestClass>(json);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testData.Id);
            result.Name.Should().Be(_testData.Name);
            result.CreatedAt.Should().Be(_testData.CreatedAt);
            result.Items.Should().BeEquivalentTo(_testData.Items);
        }

        [Fact]
        public void Deserialize_WithInvalidJson_ShouldThrowException()
        {
            // Act & Assert
            Action act = () => _serializer.Deserialize<TestClass>("invalid json");
            act.Should().Throw<Exception>();
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
            public List<string> Items { get; set; }
        }
    }
}
