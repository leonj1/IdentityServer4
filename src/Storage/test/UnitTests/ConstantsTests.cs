using Xunit;
using IdentityServer4;

namespace UnitTests
{
    public class ConstantsTests
    {
        [Fact]
        public void IdentityServerConstants_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.Equal("IdentityServer4", Constants.IdentityServerName);
            Assert.Equal("IdentityServer4", Constants.IdentityServerAuthenticationType);
        }
    }
}
