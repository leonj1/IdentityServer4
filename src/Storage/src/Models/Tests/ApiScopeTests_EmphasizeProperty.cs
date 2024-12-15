using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ApiScopeTests_EmphasizeProperty
    {
        [Fact]
        public void EmphasizeProperty_DefaultValue_ShouldBeFalse()
        {
            // Arrange
            var scope = new ApiScope("test");

            // Assert
            Assert.False(scope.Emphasize);
        }
    }
}
