using IdentityServer4.Models;
using System.Security.Claims;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientClaimTests
    {
        [Fact]
        public void Equals_WithNullObject_ShouldReturnFalse()
        {
            var claim = new ClientClaim("type1", "value1");
            Assert.False(claim.Equals(null));
        }

        [Fact]
        public void Equals_WithSameValues_ShouldReturnTrue()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type1", "value1", "valueType1");
            Assert.True(claim1.Equals(claim2));
        }

        [Fact]
        public void Equals_WithDifferentValues_ShouldReturnFalse()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type2", "value1", "valueType1");
            Assert.False(claim1.Equals(claim2));
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type1", "value1", "valueType1");
            Assert.Equal(claim1.GetHashCode(), claim2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ShouldBeDifferentForDifferentValues()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type2", "value2", "valueType2");
            Assert.NotEqual(claim1.GetHashCode(), claim2.GetHashCode());
        }
    }
}
