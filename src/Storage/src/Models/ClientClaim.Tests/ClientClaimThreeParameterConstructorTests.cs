using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientClaimThreeParameterConstructorTests
    {
        [Fact]
        public void ThreeParameterConstructor_ShouldSetAllProperties()
        {
            var claim = new ClientClaim("type1", "value1", "customValueType");
            Assert.Equal("type1", claim.Type);
            Assert.Equal("value1", claim.Value);
            Assert.Equal("customValueType", claim.ValueType);
        }
    }
}
