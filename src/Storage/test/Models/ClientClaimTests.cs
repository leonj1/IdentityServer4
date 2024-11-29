using System;
using System.Security.Claims;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientClaimTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            var claim = new ClientClaim();
            
            Assert.Null(claim.Type);
            Assert.Null(claim.Value);
            Assert.Equal(ClaimValueTypes.String, claim.ValueType);
        }

        [Fact]
        public void TwoParameterConstructor_ShouldSetValues()
        {
            var claim = new ClientClaim("type1", "value1");
            
            Assert.Equal("type1", claim.Type);
            Assert.Equal("value1", claim.Value);
            Assert.Equal(ClaimValueTypes.String, claim.ValueType);
        }

        [Fact]
        public void ThreeParameterConstructor_ShouldSetAllValues()
        {
            var claim = new ClientClaim("type1", "value1", "customValueType");
            
            Assert.Equal("type1", claim.Type);
            Assert.Equal("value1", claim.Value);
            Assert.Equal("customValueType", claim.ValueType);
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
        public void Equals_WithNull_ShouldReturnFalse()
        {
            var claim = new ClientClaim("type1", "value1", "valueType1");

            Assert.False(claim.Equals(null));
        }

        [Fact]
        public void GetHashCode_SameValues_ShouldReturnSameHash()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type1", "value1", "valueType1");

            Assert.Equal(claim1.GetHashCode(), claim2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentValues_ShouldReturnDifferentHash()
        {
            var claim1 = new ClientClaim("type1", "value1", "valueType1");
            var claim2 = new ClientClaim("type2", "value2", "valueType2");

            Assert.NotEqual(claim1.GetHashCode(), claim2.GetHashCode());
        }
    }
}
