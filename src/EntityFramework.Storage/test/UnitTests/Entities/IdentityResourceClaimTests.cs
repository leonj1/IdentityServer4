using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Entities
{
    public class IdentityResourceClaimTests
    {
        [Fact]
        public void IdentityResourceClaim_Properties_Should_Be_Settable()
        {
            // Arrange
            var identityResource = new IdentityResource();
            var claim = new IdentityResourceClaim
            {
                IdentityResourceId = 123,
                IdentityResource = identityResource,
                Type = "test-claim-type"
            };

            // Assert
            Assert.Equal(123, claim.IdentityResourceId);
            Assert.Same(identityResource, claim.IdentityResource);
            Assert.Equal("test-claim-type", claim.Type);
        }

        [Fact]
        public void IdentityResourceClaim_Should_Inherit_From_UserClaim()
        {
            // Arrange
            var claim = new IdentityResourceClaim();

            // Assert
            Assert.True(claim is UserClaim);
        }
    }
}
