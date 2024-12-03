// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Infrastructure
{
    public class ObjectSerializerTests
    {
        private class TestObject
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public bool? NullableProp { get; set; }
        }

        [Fact]
        public void Can_serialize_and_deserialize_object()
        {
            var obj = new TestObject
            {
                StringProp = "test",
                IntProp = 123,
                NullableProp = true
            };

            var serialized = IdentityServer4.ObjectSerializer.ToString(obj);
            var deserialized = IdentityServer4.ObjectSerializer.FromString<TestObject>(serialized);

            deserialized.StringProp.Should().Be("test");
            deserialized.IntProp.Should().Be(123);
            deserialized.NullableProp.Should().BeTrue();
        }

        [Fact]
        public void Serialization_ignores_null_values()
        {
            var obj = new TestObject
            {
                StringProp = "test",
                IntProp = 123,
                NullableProp = null
            };

            var serialized = IdentityServer4.ObjectSerializer.ToString(obj);
            
            serialized.Should().NotContain("NullableProp");
        }

        [Fact]
        public void Can_deserialize_message()
        {
            Action a = () => IdentityServer4.ObjectSerializer.FromString<Message<ErrorMessage>>("{\"created\":0, \"data\": {\"error\": \"error\"}}");
            a.Should().NotThrow();
        }

        [Fact]
        public void Throws_on_invalid_json()
        {
            Action a = () => IdentityServer4.ObjectSerializer.FromString<TestObject>("invalid json");
            a.Should().Throw<JsonException>();
        }

        [Fact]
        public void Can_serialize_empty_object()
        {
            var obj = new TestObject();
            var serialized = IdentityServer4.ObjectSerializer.ToString(obj);
            var deserialized = IdentityServer4.ObjectSerializer.FromString<TestObject>(serialized);

            deserialized.StringProp.Should().BeNull();
            deserialized.IntProp.Should().Be(0);
            deserialized.NullableProp.Should().BeNull();
        }

        [Fact]
        public void Can_handle_special_characters()
        {
            var obj = new TestObject
            {
                StringProp = "test\"with'special\tchars\n",
                IntProp = 456,
                NullableProp = false
            };

            var serialized = IdentityServer4.ObjectSerializer.ToString(obj);
            var deserialized = IdentityServer4.ObjectSerializer.FromString<TestObject>(serialized);

            deserialized.StringProp.Should().Be("test\"with'special\tchars\n");
            deserialized.IntProp.Should().Be(456);
            deserialized.NullableProp.Should().BeFalse();
        }

        [Fact]
        public void Throws_on_null_input()
        {
            Action a = () => IdentityServer4.ObjectSerializer.ToString(null);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_on_empty_string_deserialization()
        {
            Action a = () => IdentityServer4.ObjectSerializer.FromString<TestObject>("");
            a.Should().Throw<JsonException>();
        }
    }
}
