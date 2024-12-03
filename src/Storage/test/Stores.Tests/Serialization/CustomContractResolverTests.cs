using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using IdentityServer4.Stores.Serialization;

namespace IdentityServer4.UnitTests.Stores.Serialization
{
    public class CustomContractResolverTests
    {
        private class TestClass
        {
            public string ReadWriteProperty { get; set; }
            public string ReadOnlyProperty { get; }
            public string WriteOnlyProperty { private get; set; }
        }

        [Fact]
        public void CreateProperties_ShouldOnlyIncludeWritableProperties()
        {
            // Arrange
            var resolver = new CustomContractResolver();
            var type = typeof(TestClass);

            // Act
            var properties = resolver.ResolveContract(type).Properties;

            // Assert
            properties.Should().HaveCount(2); // ReadWriteProperty and WriteOnlyProperty
            properties.Select(p => p.PropertyName).Should().Contain("ReadWriteProperty");
            properties.Select(p => p.PropertyName).Should().Contain("WriteOnlyProperty");
            properties.Select(p => p.PropertyName).Should().NotContain("ReadOnlyProperty");
        }
    }
}
