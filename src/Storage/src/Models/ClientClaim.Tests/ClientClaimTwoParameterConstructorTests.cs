using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientClaimTwoParameterConstructorTests
    {
        [Fact]
        public void TwoParameterConstructor_ShouldSetTypeAndValue()
        {
            var claim = new ClientClaim("type1", "value1");
            Assert.Equal("type1", claim.Type);
            Assert.Equal("value1", claim.Value);
            Assert.Equal(ClaimValueTypes.String, claim.ValueType);
        }
    }
}
