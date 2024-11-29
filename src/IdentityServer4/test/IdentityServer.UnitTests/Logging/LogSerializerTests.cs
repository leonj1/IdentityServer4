using System;
using System.Text.Json;
using FluentAssertions;
using IdentityServer4.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Logging
{
    public class LogSerializerTests
    {
        [Fact]
        public void Serialize_Should_Handle_Simple_Object()
        {
            var testObject = new { Name = "Test", Value = 123 };
            
            var result = LogSerializer.Serialize(testObject);

            result.Should().Contain("\"Name\": \"Test\"");
            result.Should().Contain("\"Value\": 123");
        }

        [Fact]
        public void Serialize_Should_Ignore_Null_Values()
        {
            var testObject = new { Name = "Test", NullValue = (string)null };
            
            var result = LogSerializer.Serialize(testObject);

            result.Should().Contain("\"Name\": \"Test\"");
            result.Should().NotContain("NullValue");
        }

        [Fact]
        public void Serialize_Should_Handle_Enums_As_Strings()
        {
            var testObject = new { Status = TestEnum.Active };
            
            var result = LogSerializer.Serialize(testObject);

            result.Should().Contain("\"Status\": \"Active\"");
        }

        private enum TestEnum
        {
            Active,
            Inactive
        }
    }
}
