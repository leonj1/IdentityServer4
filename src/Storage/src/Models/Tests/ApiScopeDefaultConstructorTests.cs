using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ApiScopeDefaultConstructorTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateValidInstance()
        {
            // Act
            var scope = new ApiScope();

            // Assert
            Assert.NotNull(scope);
            Assert.Null(scope.Name);
            Assert.False(scope.Required);
            Assert.False(scope.Emphasize);
        }
    }
}
