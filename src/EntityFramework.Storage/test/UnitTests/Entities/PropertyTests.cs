using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Entities
{
    public class PropertyTests
    {
        [Fact]
        public void Property_SetAndGetProperties_ShouldWork()
        {
            // Arrange
            var testProperty = new TestProperty
            {
                Id = 1,
                Key = "TestKey",
                Value = "TestValue"
            };

            // Assert
            Assert.Equal(1, testProperty.Id);
            Assert.Equal("TestKey", testProperty.Key);
            Assert.Equal("TestValue", testProperty.Value);
        }

        private class TestProperty : Property
        {
            // Concrete implementation of abstract class for testing
        }
    }
}
