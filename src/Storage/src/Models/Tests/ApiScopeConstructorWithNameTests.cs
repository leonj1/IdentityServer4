using IdentityServer4.Models;
using System.Linq;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ApiScopeConstructorWithNameTests
    {
        [Fact]
        public void ConstructorWithName_ShouldSetNameAndDisplayName()
        {
            // Arrange
            var name = "testScope";

            // Act
            var scope = new ApiScope(name);

            // Assert
            Assert.Equal(name, scope.Name);
            Assert.Equal(name, scope.DisplayName);
            Assert.Empty(scope.UserClaims);
        }
    }
}
