using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientClaimDefaultConstructorTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValueType()
        {
            var claim = new ClientClaim();
            Assert.Equal(ClaimValueTypes.String, claim.ValueType);
        }
    }
}
